Public Class revokeauth
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete
        Try
            Dim appid As String = Page.Request.QueryString.Item("appid")
            If appid Is Nothing OrElse appid.Trim = "" Then
                Throw New Exception("bad appid")
            End If
            Dim uInfo As UserInfo = CType(Me.Master, portal).UInfo
            RemoveAuthorization(uInfo, appid.Trim)
        Catch ex As Exception
        End Try
        Response.Redirect("/portal/authlist")
    End Sub

End Class