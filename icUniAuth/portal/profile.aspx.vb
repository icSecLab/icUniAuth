Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports MySql.Data.MySqlClient

Public Class profile
    Inherits System.Web.UI.Page

    Protected Sub Page_LoadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete
        lab_id.Text = "UID:" & CType(Me.Master, portal).UInfo.UserID
        lab_email.Text = "邮箱:" & CType(Me.Master, portal).UInfo.Email
        input_nickname.Text = CType(Me.Master, portal).UInfo.NickName
        img_profile.ImageUrl = CType(Me.Master, portal).ProfileImgURL
    End Sub

    Protected Sub btn_submit_Click(sender As Object, e As EventArgs) Handles btn_submit.Click
        lab_error.Text = ""
        Try
            Dim openID As String = GetUserOpenID(CType(Me.Master, portal).UInfo)
            Dim adavterUrl As String = Nothing
            If upload_profile_img.HasFile Then
                adavterUrl = Request.Url.Scheme & "://" & Request.Url.Authority & "/portal/profileimage/" & openID & ".jpg"
                Dim profileImg As New Bitmap(upload_profile_img.FileContent)
                Dim ms As New MemoryStream
                profileImg.Save(ms, ImageFormat.Jpeg)
                If ms.Length > 1024 * 50 Then
                    Throw New Exception("图像不得大于50kB")
                End If
                File.WriteAllBytes(ServerAppRootPath & "/portal/profileimage/" & openID & ".jpg", ms.ToArray)
            End If

            Dim nickname As String = If(input_nickname.Text.Trim = "", "(null)", input_nickname.Text.Trim)
            Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
            cmd.CommandText = "update `user` set `nick_name` = @nick_name , `avatar_url` = @avatar_url where id = @id"
            cmd.Parameters.AddWithValue("@nick_name", GetBase64FromStringAsUtf8Format(nickname))
            cmd.Parameters.AddWithValue("@id", CType(Me.Master, portal).UInfo.UserID)
            cmd.Parameters.AddWithValue("@avatar_url", adavterUrl)
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            '刷新页面
            Response.AddHeader("Refresh", "0")
        Catch ex As Exception
            lab_error.Text &= ex.Message & "<br>"
        End Try
    End Sub

End Class