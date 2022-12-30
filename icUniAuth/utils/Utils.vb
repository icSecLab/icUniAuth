Imports System.Security.Cryptography

Module Utils

    Public Function GetUserOpenID(info As UserInfo) As String
        Return info.OpenID
    End Function
    Public Function GetBase64FromStringAsUtf8Format(input As String) As String
        Try
            Return Convert.ToBase64String(Text.Encoding.UTF8.GetBytes(input))
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function GetStringAsUtf8FormatFromBase64(input As String) As String
        Try
            Return Text.Encoding.UTF8.GetString(Convert.FromBase64String(input))
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Sub SendEmail(toAddr As String, bodyhtml As String)
        Dim addr As New Net.Mail.MailAddress(toAddr)
        Dim smtpClient As New Net.Mail.SmtpClient("host", 25)
        Dim mail As New Net.Mail.MailMessage()
        mail.Subject = "icUniAuth确认邮件"
        mail.From = New Net.Mail.MailAddress("noreply@icseclab.org", "icUniAuth账号事务局")
        mail.Priority = Net.Mail.MailPriority.Normal
        mail.IsBodyHtml = True
        mail.Body = bodyhtml
        mail.To.Add(addr)
        smtpClient.Send(mail)
    End Sub
    Dim GlobalRnd As Random = New Random()
    Public Function GetRandomString(length As Integer) As String
        Dim ret As String = Nothing, str As String = ""
        str += "0123456789"
        str += "abcdefghijklmnopqrstuvwxyz"
        str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        ' 妙啊，一次从 str 中随机选一个字符，把该字符拼接到 ret 末尾，太妙了
        ' 但是，length 必须是多少才能避免碰撞捏？有没有数学证明捏？
        For i As Integer = 0 To length - 1
            ret += str.Substring(GlobalRnd.[Next](0, str.Length - 1), 1)
        Next
        Threading.Thread.Sleep(1) ' wtf? 是为了什么捏？防止出现同一个字符串吗
        Return ret
    End Function

    Public Function SHA256(data As String) As String
        Dim sha As SHA256 = System.Security.Cryptography.SHA256.Create()
        Dim bytHash() As Byte
        bytHash = sha.ComputeHash(Text.Encoding.UTF8.GetBytes(data))
        sha.Clear()
        Return Bytes2String(bytHash).ToLower
    End Function
    Public Function SHA1(data As String) As String
        Dim sha As SHA1 = System.Security.Cryptography.SHA1.Create()
        Dim bytHash() As Byte
        bytHash = sha.ComputeHash(Text.Encoding.UTF8.GetBytes(data))
        sha.Clear()
        Return Bytes2String(bytHash).ToLower
    End Function
    Public Function Bytes2String(A As Byte()) As String
        Dim sb As StringBuilder = New StringBuilder()
        For i As Integer = 0 To A.Length - 1
            sb.Append(A(i).ToString("x2"))
        Next

        Return sb.ToString()
    End Function
End Module
