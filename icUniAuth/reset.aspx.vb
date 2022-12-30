Imports MySql.Data.MySqlClient

Public Class reset
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.Request.Cookies("session_id") IsNot Nothing AndAlso Page.Request.Cookies("session_id").Value.Trim <> "" Then
            If VerifySessionIDAndGetUserInfo(SHA256(Page.Request.Cookies("session_id").Value), Request.UserHostAddress).Valid Then
                Response.Redirect("/portal/reset")
            Else
                Response.Redirect("/logoff")
            End If
        End If

    End Sub
    Public Sub rst_click()
        If input_username.Text.Trim <> "" Then

            Try

                Dim usernameBase64 As String = GetBase64FromStringAsUtf8Format(input_username.Text.Trim)

                Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
                cmd.CommandText = "select `id` from `user` where `email`= @email"
                cmd.Parameters.AddWithValue("@email", usernameBase64)
                Dim dbReader As MySqlDataReader = cmd.ExecuteReader()
                cmd.Parameters.Clear()
                If dbReader.HasRows Then
                    dbReader.Read()
                    Dim uid As UInteger = dbReader("id")
                    dbReader.Close()
                    Dim activekey As String = GetRandomString(64)
                    cmd.CommandText = "
delete from `reset_pending` where `user_id`=@user_id;
insert into `reset_pending` (`user_id`,`key`) values(@user_id,@key);
"
                    cmd.Parameters.AddWithValue("@user_id", uid)
                    cmd.Parameters.AddWithValue("@key", SHA256(activekey))
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    SendEmail(input_username.Text.Trim,
                      "<a href=""" & ServerBaseUrl & "/setpassword?key=" & activekey & """>重置</a>")
                    Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Success(""处理完成，请前往邮箱确认"",""default"")", True)

                Else
                    dbReader.Close()
                    Throw New Exception("无此用户")
                End If

            Catch ex As Exception
                Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""" & RemoveDangerousCharacter(ex.Message) & """)", True)
            End Try

        Else
            Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""请填写信息"")", True)
        End If

    End Sub
End Class