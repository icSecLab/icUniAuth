Imports MySql.Data.MySqlClient

Public Class portal
    Inherits System.Web.UI.MasterPage

    Public UInfo As UserInfo
    Public ProfileImgURL As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.Request.Cookies("session_id") IsNot Nothing AndAlso Page.Request.Cookies("session_id").Value.Trim <> "" Then
            UInfo = VerifySessionIDAndGetUserInfo(SHA256(Page.Request.Cookies("session_id").Value), Request.UserHostAddress)
            If UInfo.Valid Then

                If UInfo.AvatarUrl IsNot Nothing Then
                    ProfileImgURL = UInfo.AvatarUrl
                Else
                    ProfileImgURL = "/portal/profileimage/avatar-" & ((UInfo.UserID Mod 4) + 1) & ".png"
                End If
                Image1.ImageUrl = ProfileImgURL
                Exit Sub

            Else
                Response.Redirect("/default?redirect=" & GetBase64FromStringAsUtf8Format(Page.Request.Url.AbsolutePath.ToString))
            End If
        Else
            Response.Redirect("/default?redirect=" & GetBase64FromStringAsUtf8Format(Page.Request.Url.AbsolutePath.ToString))
        End If
    End Sub

End Class