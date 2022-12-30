Imports System.IO
Imports System.Web.Routing
Imports System.Web.SessionState
Imports Microsoft.AspNet.FriendlyUrls

Public Class Global_asax
    Inherits System.Web.HttpApplication


    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        InitServer(Server.MapPath(""))
        RouteTable.Routes.EnableFriendlyUrls()
        If Not Directory.Exists(ServerAppRootPath & "/portal/profileimage/") Then
            Directory.CreateDirectory(ServerAppRootPath & "/portal/profileimage/")
        End If
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        DatabaseConnectionWatchingDog.Enabled = False
        If MySQLDatabaseConnection IsNot Nothing Then
            MySQLDatabaseConnection.Close()
            MySQLDatabaseConnection = Nothing
        End If
    End Sub

End Class