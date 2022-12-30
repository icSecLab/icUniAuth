Imports MySql.Data.MySqlClient
Imports Org.BouncyCastle.Asn1.Ocsp

Public Module LoginUtils

    Public Structure UserInfo ' 对应 `user` 表
        Public Valid As Boolean ' 这个时候不写注释的 privilege 就尽数体现了，鬼知道这个对应的是啥
        Public UserID As UInteger ' id
        Public NickName As String ' nick_name
        Public Email As String ' email
        Public OpenID As String
        Public AvatarUrl As String
    End Structure

    Public Sub RegisterUser(useremail As String, password As String)
        Dim usernameBase64 As String = GetBase64FromStringAsUtf8Format(useremail)
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "select * from user where `email`= @email"
        cmd.Parameters.AddWithValue("@email", usernameBase64)
        Dim dbReader As MySqlDataReader = cmd.ExecuteReader()
        cmd.Parameters.Clear()
        If dbReader.HasRows AndAlso dbReader.Read() Then
            dbReader.Close()
            Throw New Exception("该用户已注册")
        Else
            dbReader.Close()
            Dim salt As String = GetRandomString(16)
            Dim activekey As String = GetRandomString(64)
            Dim passwordHashwithSalt As String = SHA256(SHA256(password) & salt)
            cmd.CommandText = "
delete from `reg_pending` where `email` = @email;
insert into `reg_pending` (`email`,`pwd_salt`,`pwd_hash`,`key`) values(@email,@pwd_salt,@pwd_hash,@key);
"
            cmd.Parameters.AddWithValue("@email", usernameBase64)
            cmd.Parameters.AddWithValue("@pwd_salt", salt)
            cmd.Parameters.AddWithValue("@pwd_hash", passwordHashwithSalt)
            cmd.Parameters.AddWithValue("@key", SHA256(activekey))
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            SendEmail(useremail,
             "<a href=""" & ServerBaseUrl & "/active?key=" & activekey & """>激活</a>")
        End If
    End Sub

    Public Sub TryDeleteSession(session_id_sha256 As String)
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "delete from `session` where `session_id`= @session_id"
        cmd.Parameters.AddWithValue("@session_id", session_id_sha256)
        cmd.ExecuteNonQuery()
        cmd.Parameters.Clear()
    End Sub

    Public Function VerifySessionIDAndGetUserInfo(session_id_sha256 As String, ip_addr As String) As UserInfo
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "select `id`,`nick_name`,`email`,`open_id`,`avatar_url` from `user` where `id` = (select user_id from `session` where `session_id`= @session_id and `ip_addr` = @ip_addr)"
        cmd.Parameters.AddWithValue("@session_id", session_id_sha256)
        cmd.Parameters.AddWithValue("@ip_addr", ip_addr)
        Dim reader As MySqlDataReader = cmd.ExecuteReader()
        cmd.Parameters.Clear()
        If reader.HasRows Then
            reader.Read()
            Dim ret As UserInfo
            With ret
                .Valid = True
                .UserID = reader("id")
                .NickName = GetStringAsUtf8FormatFromBase64(reader("nick_name"))
                .Email = GetStringAsUtf8FormatFromBase64(reader("email"))
                .OpenID = reader("open_id")
                .AvatarUrl = If(IsDBNull(reader("avatar_url")), Nothing, reader("avatar_url"))
            End With
            reader.Close()
            Return ret
        End If
        reader.Close()
        Return Nothing
    End Function

    Public Function GetUserInfoByID(uid As UInteger) As UserInfo
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "select `id`,`nick_name`,`email`,`open_id`,`avatar_url` from `user` where `id` = @id"
        cmd.Parameters.AddWithValue("@id", uid)
        Dim reader As MySqlDataReader = cmd.ExecuteReader()
        cmd.Parameters.Clear()
        If reader.HasRows Then
            reader.Read()
            Dim ret As UserInfo
            With ret
                .Valid = True
                .UserID = reader("id")
                .NickName = GetStringAsUtf8FormatFromBase64(reader("nick_name"))
                .Email = GetStringAsUtf8FormatFromBase64(reader("email"))
                .OpenID = reader("open_id")
                .AvatarUrl = If(IsDBNull(reader("avatar_url")), Nothing, reader("avatar_url"))
            End With
            reader.Close()
            Return ret
        End If
        reader.Close()
        Return Nothing
    End Function
End Module
