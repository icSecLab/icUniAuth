Imports MySql.Data.MySqlClient

Public Class authlog
    Inherits System.Web.UI.Page

    Protected Sub btn_clear_Click(sender As Object, e As EventArgs) Handles btn_clear.Click
        ClearLogInfo(CType(Me.Master, portal).UInfo)
    End Sub
End Class