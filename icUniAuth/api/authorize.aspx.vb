Imports System.Threading
Imports System.Web.Management
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class authorize
    Inherits System.Web.UI.Page

    Public ResponseType, CID, RedirectUri, Scope, State As String
    Public UInfo As UserInfo
    Public AuthInfo As AuthState
    Public AppInfo As AppInfo
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Do
            If Page.Request.Cookies("session_id") IsNot Nothing AndAlso Page.Request.Cookies("session_id").Value.Trim <> "" Then
                UInfo = VerifySessionIDAndGetUserInfo(SHA256(Page.Request.Cookies("session_id").Value), Request.UserHostAddress)
                If UInfo.Valid Then
                    Exit Do
                Else
                    Response.Redirect("/default?redirect=" & GetBase64FromStringAsUtf8Format(Page.Request.Url.AbsolutePath.ToString & Page.Request.Url.Query.ToString))
                End If
            Else
                Response.Redirect("/default?redirect=" & GetBase64FromStringAsUtf8Format(Page.Request.Url.AbsolutePath.ToString & Page.Request.Url.Query.ToString))
            End If
        Loop While False

        Try
            GetRequestInfo()


            AppInfo = GetAppInfo(CID)
            If Not AppInfo.Valid Then
                Throw New AuthRequestException("invalid_request")
            End If
            If ResponseType <> "code" Then
                Throw New AuthRequestException("unauthorized_client")
            End If
            If RedirectUri <> AppInfo.RedirectUrl Then
                Throw New AuthRequestException("access_denied")
            End If
            If CID <> AppInfo.AppID Then
                Throw New AuthRequestException("access_denied")
            End If


            For Each c As Char In Scope
                If AllowedScope.IndexOf(c) < 0 Then
                    Throw New AuthRequestException("invalid_scope")
                End If
            Next
            If Scope.IndexOf("L") >= 0 Then
                Scope = "L"
            End If
            Dim tc As Char() = Scope.Distinct().ToArray
            Array.Sort(tc)
            Scope = New String(tc)

            AuthInfo = GetAuthState(UInfo.UserID, AppInfo.AppID)
            If AuthInfo = AuthState.Auto Then
                ReturnAuthCode()
            Else
                For Each c As Char In Scope
                    Select Case c
                        Case "L"
                            auth_info.Text &= "&nbsp;&nbsp;&nbsp;&nbsp;●仅登录确认(不包含您的任何信息)<br>"
                        Case "B"
                            auth_info.Text &= "&nbsp;&nbsp;&nbsp;&nbsp;●您的基本信息(OpenID,昵称)<br>"
                        Case "E"
                            auth_info.Text &= "&nbsp;&nbsp;&nbsp;&nbsp;●您的邮箱<br>"
                        Case "A"
                            auth_info.Text &= "&nbsp;&nbsp;&nbsp;&nbsp;●您的头像<br>"
                    End Select
                Next
                user_name.Text = UInfo.NickName & " (UID:" & UInfo.UserID & ")"
                app_name.Text = AppInfo.AppName & " (" & New Uri(AppInfo.RedirectUrl).Host & ")"
            End If
        Catch auth_ex As AuthRequestException
            If AppInfo.Valid Then
                Response.Redirect(AppInfo.RedirectUrl & "?error=" & auth_ex.Message & If(State Is Nothing, "", "&state=" & State))
            Else
                Response.StatusCode = 400
                Response.Charset = "UTF-8"
                Response.ContentType = "application/json"
                Response.AddHeader("Pragma", "no-cache") ' Pragma 没有提供内建的，自己添加
                Dim respJO As New JObject()
                respJO.Add("error", auth_ex.Message)
                Response.Clear()
                Response.Write(respJO.ToString(Formatting.Indented))
                Response.Flush()
                Response.Close()
            End If
        Catch tex As ThreadAbortException
            Return
        Catch ex As Exception
            Response.StatusCode = 400
            Response.Charset = "UTF-8"
            Response.ContentType = "application/json"
            Response.AddHeader("Pragma", "no-cache") ' Pragma 没有提供内建的，自己添加
            Dim respJO As New JObject()
            respJO.Add("error", ex.Message)
            Response.Clear()
            Response.Write(respJO.ToString(Formatting.Indented))
            Response.Flush()
            Response.Close()
        End Try
    End Sub

    Public AllowedScope As String = "ABEL"
    Private Sub GetRequestInfo()
        ResponseType = Page.Request.QueryString.Item("response_type")
        CID = Page.Request.QueryString.Item("client_id")
        RedirectUri = Page.Request.QueryString.Item("redirect_uri")
        Scope = Page.Request.QueryString.Item("scope")
        State = Page.Request.QueryString.Item("state")

        If Scope Is Nothing OrElse Scope.Trim = "" Then
            Scope = "L"
        End If

        If ResponseType Is Nothing OrElse ResponseType.Trim = "" Then
            Throw New AuthRequestException("invalid_request")
        End If
        If CID Is Nothing OrElse CID.Trim = "" Then
            Throw New AuthRequestException("invalid_request")
        End If
        If RedirectUri Is Nothing OrElse RedirectUri.Trim = "" Then
            Throw New AuthRequestException("invalid_request")
        End If
    End Sub

    Public Sub auth_click()
        ReturnAuthCode()
    End Sub

    Public Sub ReturnAuthCode()
        Dim code As String = CreateAuthCode(UInfo, AppInfo, AuthInfo, Scope)
        Response.Redirect(AppInfo.RedirectUrl & "?code=" & code & If(State Is Nothing, "", "&state=" & State))
    End Sub

    Public Sub notauth_click()
        Response.Redirect(AppInfo.RedirectUrl & "?error=access_denied" & If(State Is Nothing, "", "&state=" & State))
    End Sub
End Class