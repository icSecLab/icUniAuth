Imports System.Data.OleDb
Imports System.Diagnostics.Eventing
Imports System.ServiceModel.Security
Imports MySql.Data.MySqlClient
Imports Org.BouncyCastle.Asn1.Ocsp

Public Class login
    Inherits System.Web.UI.Page

    Public redirect As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        redirect = If(Page.Request.Item("redirect") Is Nothing OrElse Page.Request.Item("redirect").Trim = "", "/portal/authlist", ServerBaseUrl & GetStringAsUtf8FormatFromBase64(Request.Item("redirect")))
        If Page.Request.Cookies("session_id") IsNot Nothing AndAlso Page.Request.Cookies("session_id").Value.Trim <> "" Then
            If VerifySessionIDAndGetUserInfo(SHA256(Page.Request.Cookies("session_id").Value), Request.UserHostAddress).Valid Then
                Response.Redirect(redirect)
            Else
                Response.Redirect(If(Page.Request.Item("redirect") Is Nothing OrElse Page.Request.Item("redirect").Trim = "", "/logoff", "/logoff?redirect=" & Page.Request.Item("redirect")))
            End If
        End If
    End Sub

    Public Sub login_click()

        If input_username.Text.Trim <> "" AndAlso input_password.Text <> "" Then
            Try

                Dim usernameBase64 As String = GetBase64FromStringAsUtf8Format(input_username.Text.Trim)

                Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
                cmd.CommandText = "select * from `user` where `email` = @email"
                cmd.Parameters.AddWithValue("@email", usernameBase64)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                cmd.Parameters.Clear()

                If reader.HasRows Then
                    reader.Read()
                    Dim uid As UInteger = reader("id")
                    Dim pwd_salt As String = reader("pwd_salt")
                    Dim pwd_hash As String = reader("pwd_hash")
                    Dim status As Integer = CInt(reader("status"))
                    Dim failed_times As UInteger = CUInt(reader("failed_times"))
                    reader.Close()

                    Dim passwordHashwithSalt As String = SHA256(SHA256(input_password.Text) & pwd_salt)
                    If passwordHashwithSalt <> pwd_hash Then
                        cmd.CommandText = "update `user` set failed_times=failed_times+1 where `email` = @email"
                        cmd.Parameters.AddWithValue("@email", usernameBase64)
                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()
                        Throw New Exception("用户名密码错误或账户未激活")
                    End If
                    If failed_times >= 5 Then
                        Throw New Exception("请重置密码解锁账号")
                    End If
                    If (status And 1) <> 0 Then
                        Throw New Exception("账户状态异常：" & status.ToString)
                    End If

                    cmd.CommandText = "update `user` set failed_times=0 where `email` = @email"
                    cmd.Parameters.AddWithValue("@email", usernameBase64)
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()

                    Dim cookie As String = GetRandomString(64)

                    'Only 1 session can create in 1 ip_addr
                    cmd.CommandText = "
delete from `session` where user_id = @user_id and ip_addr = @ip_addr;
insert into `session` (session_id,user_id,ip_addr) values (@session_id,@user_id,@ip_addr);
"
                    cmd.Parameters.AddWithValue("@session_id", SHA256(cookie))
                    cmd.Parameters.AddWithValue("@user_id", uid)
                    cmd.Parameters.AddWithValue("@ip_addr", Request.UserHostAddress)
                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()

                    Dim respCookie As New HttpCookie("")
                    respCookie.Item("session_id") = cookie
                    respCookie.Expires = Now.AddHours(24)
                    Response.Cookies.Add(respCookie)

                    Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Success(""登录成功"")", True)
                Else
                    reader.Close()
                    Throw New Exception("用户名密码错误或账户未激活")
                End If
            Catch ex As Exception
                Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""" & RemoveDangerousCharacter(ex.Message) & """)", True)
            End Try
        Else
            Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""请输入用户名和密码"")", True)
        End If


    End Sub

End Class