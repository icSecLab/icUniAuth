
Imports MySql.Data.MySqlClient

Public Class manageapp
    Inherits System.Web.UI.Page

    Public appid As String
    Public AppInfo As AppInfo

    Private Sub GetRequestInfo()
        appid = Page.Request.QueryString.Item("id")
        If appid Is Nothing OrElse appid.Trim = "" Then
            Throw New Exception("invalid_app_id")
        End If
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            GetRequestInfo()
            AppInfo = GetAppInfo(appid)
        Catch ex As Exception
            Response.Redirect("/portal/myapp")
        End Try

        lab_secret.Text = AppInfo.ClientSecret
        lab_appid.Text = AppInfo.AppID
    End Sub

    Protected Sub Page_LoadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete
        Try
            If Not AppInfo.Valid OrElse AppInfo.Owner <> CType(Me.Master, portal).UInfo.UserID Then
                Throw New Exception("invalid_app_id")
            End If
        Catch ex As Exception
            Response.Redirect("/portal/myapp")
        End Try
        input_appname.Text = AppInfo.AppName
        input_redirect.Text = AppInfo.RedirectUrl
    End Sub


    Protected Sub btn_submit_Click(sender As Object, e As EventArgs) Handles btn_submit.Click
        Dim name As String = input_appname.Text
        Dim redirect_url As String = input_redirect.Text
        Try
            If redirect_url.ToLower.IndexOf("http://") <> 0 AndAlso redirect_url.ToLower.IndexOf("https://") <> 0 Then
                Throw New Exception("*重定向仅支持http,https协议")
            End If
            If name.Trim <> "" AndAlso redirect_url.Trim <> "" Then

                name = GetBase64FromStringAsUtf8Format(name)
                redirect_url = GetBase64FromStringAsUtf8Format(redirect_url)
                Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()

                Dim appid_t = GetBase64FromStringAsUtf8Format(appid)

                Dim UInfo As UserInfo = CType(Me.Master, portal).UInfo
                Dim owner As UInteger = UInfo.UserID
                cmd.Parameters.AddWithValue("@owner", owner)
                cmd.Parameters.AddWithValue("@id", appid_t)
                cmd.Parameters.AddWithValue("@name", name)
                cmd.Parameters.AddWithValue("@redirect_url", redirect_url)
                cmd.CommandText = "update `app` set `name`=@name,`redirect_url`=@redirect_url where `owner`=@owner and `id`=@id"
                cmd.ExecuteNonQuery()
                cmd.Parameters.Clear()
                Response.Redirect("/portal/myapp")

            Else
                Throw New Exception("*请输入信息")
            End If
        Catch ex As Exception
            lab_error.Text = ex.Message
        End Try
    End Sub



    Protected Sub reset_Click(sender As Object, e As EventArgs) Handles reset.Click
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        Dim client_secret As String = GetRandomString(32) '随机重置
        cmd.Parameters.AddWithValue("@owner", CType(Me.Master, portal).UInfo.UserID)
        cmd.Parameters.AddWithValue("@id", GetBase64FromStringAsUtf8Format(AppInfo.AppID))
        cmd.Parameters.AddWithValue("@client_secret", GetBase64FromStringAsUtf8Format(client_secret))
        cmd.CommandText = "update `app` set `client_secret`=@client_secret where `owner`=@owner and `id`=@id"
        cmd.ExecuteNonQuery()
        cmd.Parameters.Clear()
        lab_secret.Text = client_secret
    End Sub

    Protected Sub btn_delete_Click(sender As Object, e As EventArgs) Handles btn_delete.Click
        Dim owner As UInteger = CType(Me.Master, portal).UInfo.UserID
        Dim tmp_appid = GetBase64FromStringAsUtf8Format(appid)
        Try
            Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
            cmd.Parameters.AddWithValue("@owner", owner)
            cmd.Parameters.AddWithValue("@id", tmp_appid)
            cmd.CommandText = "delete from `app` where `owner`=@owner and `id`=@id"
            cmd.ExecuteNonQuery()
            Response.Redirect("/portal/myapp")
        Catch ex As Exception
            lab_error.Text = ex.Message
        End Try
    End Sub

    Protected Sub btn_exit_Click(sender As Object, e As EventArgs) Handles btn_exit.Click
        Response.Redirect("/portal/myapp")
    End Sub
End Class
