
Imports MySql.Data.MySqlClient

Public Class myapp
    Inherits System.Web.UI.Page

    Public x As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete
        x = ""
        Dim applist As List(Of String) = GetHTMLTable(CType(Me.Master, portal).UInfo)
        For i = 0 To applist.Count - 1
            x &= applist(i)
        Next

    End Sub
    Public Function GetHTMLTable(UInfo As UserInfo) As List(Of String)
        Dim l As New List(Of String)
        Try
            Dim cmd As MySqlCommand = MySQLDatabaseConnection.CreateCommand()
            Dim owner As UInteger = UInfo.UserID
            cmd.Parameters.AddWithValue("@owner", owner)
            cmd.CommandText = "select * from `app` where `owner`=@owner"
            Dim reader As MySqlDataReader = cmd.ExecuteReader()
            cmd.Parameters.Clear()
            While reader.Read()
                Dim id As String = reader("id")
                id = GetStringAsUtf8FormatFromBase64(id)
                Dim name As String = reader("name")
                name = GetStringAsUtf8FormatFromBase64(name)
                Dim redirect_url As String = reader("redirect_url")
                redirect_url = GetStringAsUtf8FormatFromBase64(redirect_url)
                l.Add(addMyAPP(id, name, redirect_url))
            End While
            reader.Close()
        Catch ex As Exception
            l.Clear()
        End Try
        Return l
    End Function
    Public Function addMyAPP(id As String, appname As String, redict_url As String) As String
        Return "<tr><td><div>" & appname & "</div></td><td><div class=""badge badge-primary"">" & id & "</div></td><td>" & redict_url & "</div></td><td><div>" & "<a href=""/portal/manageapp?id=" & id & """ class= ""btn btn-primary"" style=""color:#ffffff"">管理</a></div></td></tr>"
    End Function


End Class
