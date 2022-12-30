Imports System.IO
Imports System.Text.Json
Imports System.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Org.BouncyCastle.Asn1.Ocsp

Public Class Token
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' rfc6749 4.1.3 指出 access token request 的参数以 "application/x-www-urlencoded" 方式发送，其实就是 post 了一个表单 或者在url里边定义

        ' 取 grant_type
        Dim grantType As String = Request.Item("grant_type")
        ' 如果没有 "grant_type"，或者有多个 "grant_type" 字段，返回错误信息
        If grantType Is Nothing OrElse grantType.Contains(",") Then
            SendErrorResponse("invalid_request")
            Return
        End If

        ' 取 auth code
        Dim authCode As String = Request.Item("code")
        ' 如果没有 "code"，或者有多个 "code" 字段，返回错误信息
        If authCode Is Nothing OrElse authCode.Contains(",") Then
            SendErrorResponse("invalid_request")
            Return
        End If

        ' 取 redirect_url
        Dim redirectUrl As String = Request.Item("redirect_url")
        ' 如果没有 "redirect_url"，或者有多个 "redirect_url" 字段，返回错误信息
        If redirectUrl Is Nothing OrElse redirectUrl.Contains(",") Then
            SendErrorResponse("invalid_request")
            Return
        End If

        ' 取 client_id
        Dim clientId As String = Request.Item("client_id")
        ' 如果没有 "client_id"，或者有多个 "client_id" 字段，返回错误信息
        If redirectUrl Is Nothing OrElse redirectUrl.Contains(",") Then
            SendErrorResponse("invalid_request")
            Return
        End If

        ' 取 client_secret
        Dim clientSecret As String = Request.Item("client_secret")
        ' 如果没有 "client_secret"，或者有多个 "client_secret" 字段，返回错误信息
        If clientSecret Is Nothing OrElse clientSecret.Contains(",") Then
            SendErrorResponse("invalid_request")
            Return
        End If

        ' 获取 access token & refresh token
        Dim tokens As String()
        Try
            tokens = CreateAccessTokenAndRefreshToken(grantType, authCode, clientSecret, redirectUrl, clientId)
        Catch ex As UnsupportedGrantType
            ' grant_type != code
            SendErrorResponse("unsupported_grant_type")
            Return
        Catch ex As InvalidClientId
            ' client_id 是一个不存在的 id
            SendErrorResponse("invalid_client")
            Return
        Catch ex As InvalidClientSecret
            ' client_secret 错误
            SendErrorResponse("invalid_client")
            Return
        Catch ex As InvalidAuthCode
            ' auth_code 错误
            SendErrorResponse("invalid_grant")
            Return
        Catch ex As InvalidRedirectUrl
            ' redirect_url 错误
            SendErrorResponse("invalid_grant")
            Return
        End Try

        ' 没有出错，给 client 返回 access token 和 refresh token（参见 rfc6749 4.1.4 & 5.1 & 7.1）
        ' 把 entity-body 的内容打包成字典
        Dim tokenDict As New JObject
        tokenDict.Add("access_token", tokens(0))
        tokenDict.Add("token_type", "Bearer")
        tokenDict.Add("expires_in", accessTokenExpireTimeInSecond.ToString())
        tokenDict.Add("refresh_token", tokens(1))
        ' 转化为 json 字符串

        Dim jsonString As String = tokenDict.ToString
        ' 清空缓冲区
        Response.Clear()
        ' 设置响应头的参数
        Response.StatusCode = 200
        Response.ContentType = "application/json"
        Response.Charset = "UTF-8"
        Response.AddHeader("Pragma", "no-cache")
        ' 写入 entity-body
        Response.Write(jsonString)
        ' 发送
        Response.Flush()
    End Sub
    Private Sub SendErrorResponse(errorMsg As String)
        ' 把数据写进字典
        Dim errorMsgDict As New JObject
        errorMsgDict.Add("error", errorMsg)
        ' 把字典序列化成 json string
        Dim jsonString As String = errorMsgDict.ToString
        ' 发送 response
        ' 清除缓冲内容
        Response.Clear()
        ' 设置请求头
        Response.StatusCode = 400
        Response.Charset = "UTF-8"
        Response.ContentType = "application/json"
        Response.AddHeader("Pragma", "no-cache") ' Pragma 没有提供内建的，自己添加
        ' 写入 entity-body
        Response.Write(jsonString)
        ' 发送
        Response.Flush()
    End Sub
End Class
