Imports MySql.Data.MySqlClient
Public Module AccreditUtils

    Public Structure LogInfo
        Public AppName As String
        Public Time As String
    End Structure
    Public Structure AuthInfo
        Public AppName As String
        Public AppID As String
        Public Auto As Boolean
    End Structure

    Public Function GetLogInfo(User As UserInfo) As List(Of LogInfo)
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "select * from auth_log where `user`= @User"
        cmd.Parameters.AddWithValue("@User", User.UserID)
        Dim ret As List(Of LogInfo) = New List(Of LogInfo)
        Dim dbReader As MySqlDataReader = cmd.ExecuteReader()
        While dbReader.Read
            Dim a As String = GetStringAsUtf8FormatFromBase64(dbReader("app_name"))
            Dim b As String = dbReader("time") 'time 是时间不需要取base64
            Dim c As LogInfo '结构体初始化不需要实例化
            c.AppName = a
            c.Time = b
            ret.Add(c)
        End While
        cmd.Parameters.Clear()
        dbReader.Close()
        Return ret
    End Function

    Public Sub ClearLogInfo(User As UserInfo)
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "delete from auth_log where `user`= @User"
        cmd.Parameters.AddWithValue("@User", User.UserID)
        cmd.ExecuteNonQuery()
    End Sub

    Public Function GetAuthorization(User As UserInfo) As List(Of AuthInfo)
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "select `name` ,auth_info.* from app join auth_info on app.`id` = auth_info.`app_id` where auth_info.`user` = @User"
        cmd.Parameters.AddWithValue("@User", User.UserID)
        Dim ret As List(Of AuthInfo) = New List(Of AuthInfo)
        Dim dbReader As MySqlDataReader = cmd.ExecuteReader()
        While dbReader.Read
            Dim c As AuthInfo
            c.AppName = GetStringAsUtf8FormatFromBase64(dbReader("name"))
            c.Auto = If(dbReader("auto") = 0, False, True)
            c.AppID = GetStringAsUtf8FormatFromBase64(dbReader("app_id"))
            ret.Add(c)
        End While
        dbReader.Close()
        Return ret
    End Function

    '删除User对 app.id 为 APPID 的授权
    Public Sub RemoveAuthorization(User As UserInfo, AppID As String)
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "
delete from auth_code where `user_id` = @User and `app_id` = @AppID;
delete from access_token where `user_id` = @User and `app_id` = @AppID;
delete from auth_info where `user` = @User and `app_id` = @AppID;"
        cmd.Parameters.AddWithValue("@User", User.UserID)
        cmd.Parameters.AddWithValue("@AppID", GetBase64FromStringAsUtf8Format(AppID))
        cmd.ExecuteNonQuery()
    End Sub

    '更改User对APPID的自动授权状态
    '给数据库中 auth_info 加了一个 auto 参数，1表示自动授权，0表示不自动授权
    Public Sub UpdateAutoAuthorization(User As UserInfo, AppID As String)
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "update auth_info set `auto` = !`auto` where `user` = @User and `app_id` = @AppID"
        cmd.Parameters.AddWithValue("@User", User.UserID)
        cmd.Parameters.AddWithValue("@AppID", GetBase64FromStringAsUtf8Format(AppID))
        cmd.ExecuteNonQuery()
    End Sub

End Module
