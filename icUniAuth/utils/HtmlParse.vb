Module HtmlParse

    Public Function RemoveDangerousCharacter(input As String) As String
        Return input _
            .Replace("""", "") _
            .Replace("<", "") _
            .Replace("%", "") _
            .Replace(">", "") _
            .Replace("$", "") _
            .Replace("*", "") _
            .Replace("/", "") _
            .Replace("\", "") _
            .Replace(vbCr, "") _
            .Replace(vbLf, "")
    End Function
End Module
