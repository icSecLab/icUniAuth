Imports MySql.Data.MySqlClient

Public Class newapp
    Inherits System.Web.UI.Page


    Protected Sub btn_submit_Click(sender As Object, e As EventArgs) Handles btn_submit.Click
        lab_error.Text = ""
        Try
            Dim name As String = input_appname.Text.Trim
            Dim redirect_url As String = redirect.Text.Trim
            If redirect_url.ToLower.IndexOf("http://") <> 0 AndAlso redirect_url.ToLower.IndexOf("https://") <> 0 Then
                Throw New Exception("*重定向仅支持http,https协议")
            End If
            If name <> "" And redirect_url <> "" Then
                Dim UInfo As UserInfo = CType(Me.Master, portal).UInfo
                Dim owner As UInteger = UInfo.UserID
                Dim id As String = GetRandomString(32)
                Dim client_secret As String = GetRandomString(32)
                Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
                cmd.CommandText = "insert into `app`(`id`,`owner`,`redirect_url`,`client_secret`,`name`) value(@id,@owner,@redirect_url,@client_secret,@name)"
                cmd.Parameters.AddWithValue("@id", GetBase64FromStringAsUtf8Format(id))
                cmd.Parameters.AddWithValue("@owner", owner)
                cmd.Parameters.AddWithValue("@redirect_url", GetBase64FromStringAsUtf8Format(redirect_url))
                cmd.Parameters.AddWithValue("@client_secret", GetBase64FromStringAsUtf8Format(client_secret))
                cmd.Parameters.AddWithValue("@name", GetBase64FromStringAsUtf8Format(name))
                cmd.ExecuteNonQuery()
                cmd.Parameters.Clear()
                Response.Redirect("/portal/myapp.aspx")

            Else
                Throw New Exception("*请填写信息")
            End If
        Catch ex As Exception
            lab_error.Text = RemoveDangerousCharacter(ex.Message)
        End Try
    End Sub
End Class