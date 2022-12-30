Public Class logoff
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim respCookie As New HttpCookie("")
        respCookie.Item("session_id") = ""
        Response.Cookies.Add(respCookie)

        If Page.Request.Cookies("session_id") IsNot Nothing AndAlso
            Page.Request.Cookies("session_id").Value IsNot Nothing AndAlso
            Page.Request.Cookies("session_id").Value.Trim <> "" Then

            TryDeleteSession(SHA256(Page.Request.Cookies("session_id").Value))
        End If

        Response.Redirect(If(Page.Request.Item("redirect") Is Nothing OrElse Page.Request.Item("redirect").Trim = "", "/default", "/default?redirect=" & Page.Request.Item("redirect")))
    End Sub

End Class