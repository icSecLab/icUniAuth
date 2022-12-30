Imports Newtonsoft.Json.Linq

Module ServerConfigUtils

    Public ServerAppRootPath As String
    Public ServerBaseUrl As String
    Public ServerConfiged As Boolean
    Public DatabaseName As String
    Public DatabaseConnectionUsername As String
    Public DatabaseConnectionPassword As String
    Public DatabaseServerHost As String
    Public DatabaseServerPort As Integer
    Public Sub GetServerConfig()
        Dim ConfigJO As JObject = JObject.Parse(IO.File.ReadAllText(ServerAppRootPath & "/server.conf"))
        ServerConfiged = ConfigJO.Item("Server").Item("Configed").ToString
        ServerBaseUrl = ConfigJO.Item("Server").Item("BaseURL").ToString
        DatabaseServerHost = ConfigJO.Item("Database").Item("Host").ToString
        DatabaseServerPort = CInt(ConfigJO.Item("Database").Item("Port"))
        DatabaseConnectionUsername = ConfigJO.Item("Database").Item("Username").ToString
        DatabaseConnectionPassword = ConfigJO.Item("Database").Item("Password").ToString
        DatabaseName = ConfigJO.Item("Database").Item("Name").ToString
    End Sub

    Public Sub InitServer(rootPath As String)
        ServerAppRootPath = rootPath
        InitServer()
    End Sub
    Public Sub InitServer()
        GetServerConfig()
        If ServerConfiged Then
            DatabaseConnectionWatchingDog.Enabled = True
            CheckDatabaseConnection()
        End If
    End Sub
End Module
