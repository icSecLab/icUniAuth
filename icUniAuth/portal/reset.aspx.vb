Imports MySql.Data.MySqlClient

Public Class reset1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btn_submit_Click(sender As Object, e As EventArgs) Handles btn_submit.Click

        Try
            Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
            Dim activekey As String = GetRandomString(64)
            cmd.CommandText = "
delete from `reset_pending` where `user_id`=@user_id;
insert into `reset_pending` (`user_id`,`key`) values(@user_id,@key);
"
            cmd.Parameters.AddWithValue("@user_id", CType(Me.Master, portal).UInfo.UserID)
            cmd.Parameters.AddWithValue("@key", SHA256(activekey))
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            SendEmail(CType(Me.Master, portal).UInfo.Email,
              "<a href=""" & ServerBaseUrl & "/setpassword?key=" & activekey & """>重置</a>")

            btn_submit.Enabled = False
            Throw New Exception("处理完成，请前往邮箱确认")
        Catch ex As Exception
            lab_error.Text = RemoveDangerousCharacter(ex.Message)
        End Try

    End Sub
End Class