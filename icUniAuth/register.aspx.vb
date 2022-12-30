Imports MySql.Data.MySqlClient

Public Class register
    Inherits System.Web.UI.Page

    Public redirect As String = "/default"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.Request.Cookies("session_id") IsNot Nothing AndAlso Page.Request.Cookies("session_id").Value.Trim <> "" Then
            If VerifySessionIDAndGetUserInfo(SHA256(Page.Request.Cookies("session_id").Value), Request.UserHostAddress).Valid Then
                Response.Redirect(redirect)
            Else
                Response.Redirect("/logoff")
            End If
        End If
    End Sub
    Public Sub reg_click()
        If input_username.Text.Trim <> "" AndAlso input_password.Text <> "" AndAlso input_repassword.Text <> "" Then
            If input_password.Text = input_repassword.Text Then
                Try

                    RegisterUser(input_username.Text.Trim, input_password.Text)
                    Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Success(""处理完成，请前往邮箱确认"",""/default"")", True)
                Catch ex As Exception
                    Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""" & RemoveDangerousCharacter(ex.Message) & """)", True)
                End Try
            Else
                Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""两次输入的密码不匹配"")", True)
            End If
        Else
            Page.ClientScript.RegisterStartupScript(Page.GetType, "", "Error(""请填写注册信息"")", True)
        End If

    End Sub

End Class