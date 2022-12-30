Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class get_user_avater
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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

            If scope.IndexOf("A") < 0 Then
                Throw New Exception("not permitted")
            End If

            Dim img As Byte() = Nothing
            Try
                If uInfo.AvatarUrl IsNot Nothing Then

                    Dim wc As New WebClientEx()
                    wc.Timeout = 3000
                    wc.KeepAlive = False
                    wc.Headers.Add(HttpRequestHeader.UserAgent, Request.UserAgent)
                    img = wc.DownloadData(uInfo.AvatarUrl)

                Else
                    Throw New Exception("avatar not found")
                End If
            Catch ex As Exception
                img = IO.File.ReadAllBytes(ServerAppRootPath & "/portal/profileimage/avatar-" & ((uInfo.UserID Mod 4) + 1) & ".png")
            End Try
            Response.ContentType = "application/jpeg"
            Response.BinaryWrite(img)
            Response.Flush()
        Catch ex As Exception
            Dim retJO As New JObject
            retJO.Add("status", 1)
            retJO.Add("msg", RemoveDangerousCharacter(ex.Message))
            Response.StatusCode = 400
            Response.ContentType = "application/json"
            Response.Write(retJO.ToString(Formatting.Indented))
            Response.Flush()
        End Try

    End Sub

End Class