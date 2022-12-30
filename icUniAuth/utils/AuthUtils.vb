Imports Google.Protobuf.WellKnownTypes
Imports Microsoft.VisualBasic.ApplicationServices
Imports MySql.Data.MySqlClient

Public Module AuthUtils
    ' 建议多写点注释，用英语来不及写就用中文写，md 审阅你的代码太费劲了，
    ' 相信我，写注释真的不会累死你，真的

    ' 为什么所有 string 类型的变量往数据库里存的时候都要换成 base64 捏？

    Public Structure AppInfo ' 部分字段对应的是 `app` 表，下面的注释内容是变量对应的字段名称
        Public Valid As Boolean ' wtf? 变量名称不是小驼峰吗？
        Public AppID As String ' id (utf-8 format)
        Public Owner As UInteger ' owner
        Public RedirectUrl As String ' redirect_url (utf-8 format)
        Public ClientSecret As String ' client_secret (utf-8 format)
        Public AppName As String ' name (utf-8 format)
    End Structure

    Public Enum AuthState ' 对应 `auth_info` 表的 auto 字段，类型是 TINYINT，NOTNULL
        NoAuthInfo ' 这个时候不写注释的 privilege 就尽数体现了，鬼知道这个对应的是啥
        Auto ' 这个时候不写注释的 privilege 就尽数体现了，鬼知道这个对应的是啥
        Manual ' auto = 0
    End Enum

    Public Function GetAppInfo(appid As String) As AppInfo
        ' 写查询语句，是 prepared statement
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "select `id`,`owner`,`redirect_url`,`client_secret`,`name` from `app` where `id` = @appid"
        ' 指定参数
        cmd.Parameters.AddWithValue("@appid", GetBase64FromStringAsUtf8Format(appid))
        ' 执行查询。建议把鼠标放上去你看一下 MySqlDataReader 的帮助信息，reader 执行时，connection 会被完全占用，一个 connection 可能不够用，
        ' 尤其当你的产品上线投入实际使用的时候，用户多起来卡死你，然后你看用户骂不骂你
        Dim reader As MySqlDataReader = cmd.ExecuteReader()
        ' 清空参数，好评
        cmd.Parameters.Clear()
        ' 读取查询结果
        If reader.HasRows Then ' 实际上这里应该返回的表里应该只有一个结果
            reader.Read()
            Dim ret As AppInfo
            With ret
                .Valid = True
                .AppID = GetStringAsUtf8FormatFromBase64(reader("id"))
                .Owner = CUInt(reader("owner"))
                .RedirectUrl = GetStringAsUtf8FormatFromBase64(reader("redirect_url"))
                .ClientSecret = GetStringAsUtf8FormatFromBase64(reader("client_secret"))
                .AppName = GetStringAsUtf8FormatFromBase64(reader("name"))
            End With
            ' 关闭 reader，好评
            reader.Close()
            Return ret
        End If
        ' 不优雅，明明写一次 close 就行的
        reader.Close()
        Return Nothing
    End Function

    Public Function GetAuthState(userid As UInteger, appid As String) As AuthState
        ' 写查询语句，是 prepared statement
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "select `auto` from `auth_info` where `user` = @userid and `app_id` = @appid"
        ' 指定参数
        cmd.Parameters.AddWithValue("@appid", GetBase64FromStringAsUtf8Format(appid))
        cmd.Parameters.AddWithValue("@userid", userid)
        ' 执行查询。建议同上
        Dim reader As MySqlDataReader = cmd.ExecuteReader()
        ' 清空参数
        cmd.Parameters.Clear()
        Dim ret As AuthState = AuthState.NoAuthInfo
        ' 读取查询结果
        If reader.HasRows Then
            reader.Read()
            ret = If(reader("auto") = 0, AuthState.Manual, AuthState.Auto)
        End If
        ' 怎么现在知道优雅了
        reader.Close()
        Return ret
    End Function

    Public Function CreateAuthCode(uinfo As UserInfo, appinfo As AppInfo, state As AuthState, scope As String) As String
        ' 生成 authorization code，它是一个一个一个长度为 64 个字符的字符串，包含字符 A-Z && a-z && 0-9
        Dim code As String = GetRandomString(64)
        ' 写查询语句
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "
insert into `auth_code` (`token`,`app_id`,`user_id`,`time`,`scope`) values (@code,@appid,@userid,@time,@scope);
insert into `auth_log` (`user`,`app_name`,`time`) values (@userid,@appname,@time);
"
        If state = AuthState.NoAuthInfo Then
            cmd.CommandText += ("insert into `auth_info` (`user`,`app_id`,`auto`) values (@userid,@appid,0);") ' 为什么要加括号捏
        End If
        cmd.Parameters.AddWithValue("@scope", GetBase64FromStringAsUtf8Format(scope))
        cmd.Parameters.AddWithValue("@code", SHA256(code)) ' authorization code 在表里是 SHA256 的摘要
        cmd.Parameters.AddWithValue("@appid", GetBase64FromStringAsUtf8Format(appinfo.AppID))
        cmd.Parameters.AddWithValue("@userid", uinfo.UserID)
        cmd.Parameters.AddWithValue("@time", Now.ToUniversalTime)
        cmd.Parameters.AddWithValue("@appname", GetBase64FromStringAsUtf8Format(appinfo.AppName))
        cmd.ExecuteNonQuery()
        Return code
    End Function
    ' 调用场景（参见 rfc6749 section 4.1.3）：当 client 向 auth server 发起 Access Token Request 时，auth server 后端调用该函数，生成 access token 和 refresh token
    ' 参数说明：
    '     全部由 request body 提供：
    '          grantTypeInRequest：rfc6749 section 4.1.3 中提到的 client request 的 grant_type
    '          authCodeInRequest：rfc6749 section 4.1.3 中提到的 client request 的 code
    '          redirectUrlInRequest：rfc6749 section 4.1.3 中提到的 client request 的 redirect_uri
    '          clientIdInRequest：rfc6749 section 4.1.3 中提到的 client request 的 client_id
    '          clientSecretInRequest：rfc6749 section 4.1.3 中提到，如果 client_type = confidential，那么就一定要进行验证（方法见 2.3 节）
    '                                 本项目目前默认 client_type = confidential，所以采用 2.3.1 节的 pwd 方法进行验证，要求 client 发送 client_secret，
    '                                 该变量即为 request body 中的 client_secret 字段
    '     这些参数一定和 client 发来的一样，因为 OAuth 基于 HTTP 和 TLS，发来的信息一定是正确、完整、保密的
    '     调用者要保证传进来的不能是 Nothing，而且参数不能重复，request body 中不能有不认识的参数
    ' 返回值说明：如果通过验证，则返回一个 String(2)，第一个元素是 access token，第二个是 refresh token

    ' 这是一个全局变量，作用于整个项目，指明 access token 的有效时间，以秒为单位
    Public Const accessTokenExpireTimeInSecond As Integer = 3600
    Public Function CreateAccessTokenAndRefreshToken(grantTypeInRequest As String, authCodeInRequest As String, clientSecretInRequest As String,
                                                     redirectUrlInRequest As String, clientIdInRequest As String) As String()
        ' 首先检查 grant_type，必须是"authorization_code"，否则删除 auth code 并抛出异常（需要调用该函数的过程捕捉错误，向客户端返回出错信息）
        If grantTypeInRequest <> "authorization_code" Then
            ' 抛出异常
            Throw New UnsupportedGrantType("Invalid grant type!")
            ' 删除 auth code
            DeleteAuthCode(authCodeInRequest)
            Return Nothing
        End If

        ' 再验证 client 的身份，根据 client_id 查 client_secret
        ' 写查询语句
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "select `client_secret` from `app` where `id` = @client_id;"
        ' 指定参数
        cmd.Parameters.AddWithValue("@client_id", GetBase64FromStringAsUtf8Format(clientIdInRequest))
        ' 执行查询
        Dim reader As MySqlDataReader = cmd.ExecuteReader()
        ' 清空参数
        cmd.Parameters.Clear()
        ' 获取结果并判断
        Dim clientSecret As String
        If reader.HasRows Then
            ' 结果不为空
            ' 获取结果
            reader.Read()
            clientSecret = reader("client_secret")
            reader.Close()
            ' 判断
        Else
            ' 结果为空
            ' 笑死，client_id 不存在，删除auth code，抛出异常（需要调用该函数的过程捕捉错误，向客户端返回出错信息）
            reader.Close()
            ' 删除 auth code
            DeleteAuthCode(authCodeInRequest)
            ' 抛出异常
            Throw New InvalidClientId("Client id does NOT exist!")
        End If

        If GetBase64FromStringAsUtf8Format(clientSecretInRequest) <> clientSecret Then ' 注意数据库里存的是 base64
            ' 客户端密码不正确，未通过验证，删除 auth code，并抛出异常（需要调用该函数的过程捕捉错误，向客户端返回出错信息）
            ' 删除 auth code
            DeleteAuthCode(authCodeInRequest)
            ' 抛出异常
            Throw New InvalidClientSecret("Invalid client secret!")
        End If

        ' 客户端密码正确，检查 auth code（4.1.3 节的规定）
        ' 写查询语句
        cmd.CommandText = "select * from `auth_code` where `token` = @auth_code;"
        ' 指定参数
        cmd.Parameters.AddWithValue("@auth_code", SHA256(authCodeInRequest)) ' 注意，查表要用 SHA256 后的摘要查
        ' 执行查询
        reader = cmd.ExecuteReader()
        ' 清空参数
        cmd.Parameters.Clear()
        ' 获取结果并判断
        Dim userID As UInteger
        Dim scope As String
        If reader.HasRows Then
            ' 结果不为空，auth code 是真的，把 user_id 和 scope 存一下，生成 access_token 和 refresh_token 后插入数据库会用到（如果可以到达这一步的话）
            reader.Read()
            userID = reader("user_id")
            scope = GetStringAsUtf8FormatFromBase64(reader("scope"))
            reader.Close()
        Else
            ' 结果为空，auth code 是假的，删除 auth code 并抛出异常（需要调用该函数的过程捕捉错误，向客户端返回出错信息）
            reader.Close()
            ' 删除 auth code
            DeleteAuthCode(authCodeInRequest)
            ' 抛出异常
            Throw New InvalidAuthCode("Invalid authorization code!")
        End If

        ' 接下来检查 redirect url 是否正确（和 app 注册时提供的 url 进行比对）
        ' 写查询语句
        cmd.CommandText = "select `redirect_url` from `app` where `id` = @client_id;"
        ' 指定参数
        cmd.Parameters.AddWithValue("@client_id", GetBase64FromStringAsUtf8Format(clientIdInRequest))
        ' 执行查询
        reader = cmd.ExecuteReader()
        ' 获取结果并比对
        Dim redirectUrl As String
        If reader.HasRows Then
            ' 结果不为空
            ' 获取结果
            reader.Read()
            redirectUrl = reader("redirect_url")
            reader.Close()
        Else
            ' 结果为空，删除 auth code，抛出异常（需要调用该函数的过程捕捉错误，向客户端返回出错信息）
            reader.Close()
            ' 删除 auth code
            DeleteAuthCode(authCodeInRequest)
            ' 抛出异常
            Throw New InvalidRedirectUrl("Invalid redirect url!")
        End If

        ' 比对结果
        If redirectUrl = GetBase64FromStringAsUtf8Format(redirectUrlInRequest) Then ' 注意数据库里存的是 base64
            ' 结果吻合，至此 client 通过验证
            ' 删除 auth code
            DeleteAuthCode(authCodeInRequest)
            ' 生成 access token
            Dim accessToken As String = GetRandomString(64)
            ' 生成 refresh token
            Dim refreshToken As String = GetRandomString(64)
            ' 打包
            Dim ret As String() = {accessToken, refreshToken}
            ' 存入数据库
            ' 写插入语句
            cmd.CommandText = "insert into `access_token`(`token`, `app_id`, `time`, `scope`, `refresh_token`, `refresh_time`, `user_id`)
                                                values (@access_token, @app_id, @time, @scope, @refresh_token, @refresh_time, @user_id);"
            ' 指定参数
            cmd.Parameters.AddWithValue("@access_token", SHA256(accessToken)) ' 注意这里，数据库里存的 access token 是哈希后的值
            cmd.Parameters.AddWithValue("@app_id", GetBase64FromStringAsUtf8Format(clientIdInRequest)) ' 注意这里存的是原字符串的base64
            cmd.Parameters.AddWithValue("@scope", GetBase64FromStringAsUtf8Format(scope)) ' scope
            cmd.Parameters.AddWithValue("@refresh_token", SHA256(refreshToken)) ' 注意这里，数据库里存的 refresh token 是哈希后的值
            cmd.Parameters.AddWithValue("@user_id", userID) ' 授权给 client 访问权限的 user 的 id
            ' 为了减小误差，我们把和和时间有关的操作放在一起执行
            Dim tempDatetime As Date = Now.ToUniversalTime ' 取当前本地的 UTC 时间，为了让 @time = @refresh_time + 3600s，我们定义这个变量
            cmd.Parameters.AddWithValue("@time", tempDatetime) ' 取当前本地的 UTC 时间
            cmd.Parameters.AddWithValue("@refresh_time", tempDatetime.AddSeconds(accessTokenExpireTimeInSecond)) ' 注意这里，对应 rfc6749 section 5.1 的 expire_in 没有完全按照协议来实现，这里的 refresh_time 是 access_token 的失效时间是当前时间 + 3600 secs
            ' 执行插入
            cmd.ExecuteNonQuery()
            ' 清除参数
            cmd.Parameters.Clear()
            Return ret
        Else
            ' 结果不吻合，删除 auth code，抛出异常（需要调用该函数的过程捕捉错误，向客户端返回出错信息）
            ' 删除 auth code
            DeleteAuthCode(authCodeInRequest)
            ' 抛出异常
            Throw New InvalidRedirectUrl("Invalid redirect url!")
            Return Nothing
        End If
    End Function
    ' ==========================从这里开始是 CreateAccessTokenAndRefreshToken() 中用到的函数和异常类==========================

    ' 调用场景：在 CreateAccessTokenAndRefreshToken() 中，某个验证失败，需要删除 auth code 时调用
    ' 参数说明：authCode：欲删除的 auth code 原文（该函数会把它转化为它的 SHA256 摘要）
    Public Sub DeleteAuthCode(authCode As String)
        ' 数据库里存的是 auth code 的 SHA256，要转换一下
        Dim authCodeSha256 = SHA256(authCode)
        ' 写 ddl 语句
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "delete from `auth_code` where `token` = @auth_code;"
        ' 指定参数
        cmd.Parameters.AddWithValue("@auth_code", authCodeSha256)
        ' 执行语句
        cmd.ExecuteNonQuery()
        ' 清空参数
        cmd.Parameters.Clear()
    End Sub
    ' 无效的 grant type
    Public Class UnsupportedGrantType : Inherits ApplicationException
        Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class
    ' 无效的 client id
    Public Class InvalidClientId : Inherits ApplicationException
        Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class
    ' 无效的 client secret
    Public Class InvalidClientSecret : Inherits ApplicationException
        Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class
    ' 无效的 auth code
    Public Class InvalidAuthCode : Inherits ApplicationException
        Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class
    ' 无效的 redirect url
    Public Class InvalidRedirectUrl : Inherits ApplicationException
        Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class
    ' ==========================CreateAccessTokenAndRefreshToken() 中用到的函数和异常类结束==========================

    ' 调用场景：client 已经取得 access token，并且使用 token 请求 protected resources，resource server 接收到 token 后，在后端调用此函数验证 token 的有效性
    '         可参见 rfc6749 1.5 节 figure 2 的步骤(D)，该函数的调用点在(D)的服务器端
    ' 参数说明：
    '         token：client 发来的 access token，需要 SHA256() 后才能用于查询数据库
    ' 返回值说明：
    '         一个 3-tuple，分量从前往后分别为：token 的 valid flag，user_id 和 scope
    '         如果 token 不存在或者存在但是过期，返回 (False, 0, "")
    '         如果 token 存在且未过期，返回 (True, user_id, scope)
    Public Function VerifyAccessTokenGetUserIdAndScope(token As String) As (Boolean, UInteger, String)
        ' 获取当前时间，后面验证 token 是否过期时会用到
        Dim currentTime = Now.ToUniversalTime()
        ' 返回值的三个分量
        Dim isValid As Boolean = False
        Dim userId As UInteger = 0
        Dim scope As String = ""
        ' 写查询语句
        Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
        cmd.CommandText = "select * from `access_token` where `token` = @access_token;"
        ' 指定参数
        cmd.Parameters.AddWithValue("@access_token", SHA256(token))
        ' 执行查询
        Dim reader As MySqlDataReader = cmd.ExecuteReader()
        ' 清除参数
        cmd.Parameters.Clear()
        ' 检查 token 是否存在
        If Not reader.HasRows Then
            ' token 不存在，结束读取并返回
            reader.Close()
            Return (isValid, userId, scope)
        End If
        ' token 存在，检查 token 是否过期
        reader.Read()
        Dim expireTime As DateTime = reader("refresh_time")
        If DateTime.Compare(currentTime, expireTime) >= 0 Then
            ' token 过期，结束读取并返回
            reader.Close()
            Return (isValid, userId, scope)
        End If
        ' token 存在并且未过期，返回查询信息
        isValid = True
        userId = reader("user_id")
        scope = reader("scope")
        reader.Close()
        Return (isValid, userId, GetStringAsUtf8FormatFromBase64(scope))
    End Function
End Module
