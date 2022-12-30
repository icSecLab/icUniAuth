Imports MySql.Data.MySqlClient

Public Class active
    Inherits System.Web.UI.Page

    Public redirect As String = "/default"
    Private key As String
    Private email As String
    Private pwd_salt As String
    Private pwd_hash As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.Request.Cookies("session_id") IsNot Nothing AndAlso Page.Request.Cookies("session_id").Value.Trim <> "" Then
            If VerifySessionIDAndGetUserInfo(SHA256(Page.Request.Cookies("session_id").Value), Request.UserHostAddress).Valid Then
                Response.Redirect(redirect)
            Else
                Response.Redirect("/logoff")
            End If
        End If

        key = Page.Request.Item("key")
        If key Is Nothing OrElse key.Trim = "" Then
            Page.ClientScript.RegisterStartupScript(Page.GetType, "", "ErrorRedirect(""激活码无效"")", True)
        Else

            Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
            cmd.CommandText = "select * from `reg_pending` where `key` = @key"
            cmd.Parameters.AddWithValue("@key", SHA256(key))
            Dim reader As MySqlDataReader = cmd.ExecuteReader()
            cmd.Parameters.Clear()

            If reader.HasRows Then
                reader.Read()
                email = reader("email")
                pwd_salt = reader("pwd_salt")
                pwd_hash = reader("pwd_hash")
            Else
                Page.ClientScript.RegisterStartupScript(Page.GetType, "", "ErrorRedirect(""激活码无效"")", True)
            End If

            Try
                reader.Close()
            Catch
            End Try

        End If
    End Sub

    Public Sub active_click()
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "delete from `reg_pending` where `key` = @key"
        cmd.Parameters.AddWithValue("@key", SHA256(key))
        Dim arow As Integer = cmd.ExecuteNonQuery()
        cmd.Parameters.Clear()

        If GetBase64FromStringAsUtf8Format(input_username.Text.Trim) = email Then
            Try
                If arow <= 0 Then
                    Throw New Exception("激活码无效")
                End If

                cmd.CommandText = "insert into `user` (`email`,`nick_name`,`pwd_salt`,`pwd_hash`,`status`,`failed_times`,`open_id`,`avatar_url`) values (@email,@nick_name,@pwd_salt,@pwd_hash,@status,@failed_times,@open_id,null)"
                cmd.Parameters.AddWithValue("@email", email)
                cmd.Parameters.AddWithValue("@nick_name", GetBase64FromStringAsUtf8Format(If(input_nickname.Text.Trim = "", "(null)", input_nickname.Text.Trim)))
                cmd.Parameters.AddWithValue("@pwd_salt", pwd_salt)
                cmd.Parameters.AddWithValue("@pwd_hash", pwd_hash)
                cmd.Parameters.AddWithValue("@status", 0)
                cmd.Parameters.AddWithValue("@failed_times", 0)
                cmd.Parameters.AddWithValue("@open_id", GetRandomString(32))
                cmd.ExecuteNonQuery()
                cmd.Parameters.Clear()
                Page.ClientScript.RegisterStartupScript(Page.GetType, "", "SuccessRedirect(""激活成功"")", True)
            Catch ex As Exception
                Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""" & ex.Message & "，请重新注册"")", True)
            End Try
        Else
            Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""邮箱不匹配，请重新注册"")", True)
        End If
    End Sub

End Class