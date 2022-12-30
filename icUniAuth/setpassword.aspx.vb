Imports System.Net
Imports MySql.Data.MySqlClient

Public Class setpassword
    Inherits System.Web.UI.Page

    Public redirect As String = "/default"
    Private key As String
    Private uid As UInteger = UInteger.MaxValue
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        key = Page.Request.Item("key")
        If key Is Nothing OrElse key.Trim = "" Then
            Page.ClientScript.RegisterStartupScript(Page.GetType, "", "ErrorRedirect(""重置码无效"")", True)
        Else
            Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
            cmd.CommandText = "select * from `reset_pending` where `key` = @key"
            cmd.Parameters.AddWithValue("@key", SHA256(key))
            Dim reader As MySqlDataReader = cmd.ExecuteReader()
            cmd.Parameters.Clear()
            If reader.HasRows Then
                reader.Read()
                uid = reader("user_id")
            Else
                Page.ClientScript.RegisterStartupScript(Page.GetType, "", "ErrorRedirect(""重置码无效"")", True)
            End If
            Try
                reader.Close()
            Catch
            End Try

        End If
    End Sub
    Public Sub submit_click()
        If input_username.Text.Trim <> "" AndAlso input_password.Text <> "" AndAlso input_repassword.Text <> "" Then
            If input_password.Text = input_repassword.Text Then
                Try

                    If uid = UInteger.MaxValue Then
                        Throw New Exception("uid无效")
                    End If

                    Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
                    cmd.CommandText = "delete from `reset_pending` where `key` = @key"
                    cmd.Parameters.AddWithValue("@key", SHA256(key))
                    Dim arow As Integer = cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()
                    If arow <= 0 Then
                        Throw New Exception("重置码无效")
                    End If


                    Dim usernameBase64 As String = GetBase64FromStringAsUtf8Format(input_username.Text.Trim)
                    Dim salt As String = GetRandomString(16)
                    Dim passwordHashwithSalt As String = SHA256(SHA256(input_password.Text) & salt)

                    cmd.CommandText = "
update `user` set pwd_salt=@pwd_salt,pwd_hash=@pwd_hash,failed_times=0 where `id` = @id and email = @email;
delete from `session` where user_id=@id;
"
                    cmd.Parameters.AddWithValue("@email", usernameBase64)
                    cmd.Parameters.AddWithValue("@pwd_salt", salt)
                    cmd.Parameters.AddWithValue("@pwd_hash", passwordHashwithSalt)
                    cmd.Parameters.AddWithValue("@id", uid)
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()


                    Page.ClientScript.RegisterStartupScript(Page.GetType, "", "SuccessRedirect(""处理完成"")", True)

                Catch ex As Exception
                    Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""" & RemoveDangerousCharacter(ex.Message) & """)", True)
                End Try
            Else
                Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""两次输入的密码不匹配"")", True)
            End If
        Else
            Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""请填写账号信息"")", True)
        End If
    End Sub
End Class