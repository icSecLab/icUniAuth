Imports System.Security.Cryptography
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class user_info
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim retJO As New JObject
        retJO.Add("status", 0)
        retJO.Add("msg", "OK")
        Dim dataJToken As New JObject
        Response.Charset = "utf-8"
        Response.AddHeader("Pragma", "no-cache")
        Try
            Dim token As String = Page.Request.Item("access_token")
            If token Is Nothing OrElse token.Trim = "" Then
                Throw New Exception("bad token")
            End If

            'VerifyAccessTokenGetUserIdAndScope函数As (Boolean, UInteger, String)
            '分别为有效指示，uid，scope
            Dim verifyResult As (Boolean, UInteger, String) = VerifyAccessTokenGetUserIdAndScope(token)

            If verifyResult.Item1 = False Then
                Throw New Exception("bad token")
            End If

            Dim uInfo As UserInfo = GetUserInfoByID(verifyResult.Item2)
            Dim scope As String = verifyResult.Item3

            For Each c As Char In scope
                Select Case c
                    Case "L"
                        dataJToken.Add("login_status", "success")
                    Case "B"
                        dataJToken.Add("nick_name", uInfo.NickName)
                        dataJToken.Add("open_id", SHA1(uInfo.OpenID))
                    Case "E"
                        dataJToken.Add("email", uInfo.Email)
                End Select
            Next
            retJO.Add("data", dataJToken)
            '不用指定 HTTPStatusCode = 200 默认为 200
        Catch ex As Exception
            Response.StatusCode = 400
            retJO.RemoveAll()
            retJO("status") = 1
            retJO("msg") = RemoveDangerousCharacter(ex.Message)
        End Try
        Response.ContentType = "application/json"
        Response.Write(retJO.ToString(Formatting.Indented))
        Response.Flush()

    End Sub

End Class