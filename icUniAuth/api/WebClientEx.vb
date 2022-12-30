'MIT License

'Copyright(c) 2021 HBSnail

'Permission Is hereby granted, free Of charge, to any person obtaining a copy
'of this software And associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
'copies of the Software, And to permit persons to whom the Software Is
'furnished to do so, subject to the following conditions:

'The above copyright notice And this permission notice shall be included In all
'copies Or substantial portions of the Software.

'THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
'IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
'LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
'OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN THE
'SOFTWARE.


Imports System.Net

Public Class WebClientEx
    Inherits WebClient
    Public Property Timeout As Integer

    Public Property KeepAlive As Boolean

    Public Property ResponseUrl As String

    Protected Overrides Function GetWebRequest(ByVal address As Uri) As WebRequest
        Dim request = MyBase.GetWebRequest(address)
        request.Timeout = Timeout
        request.Proxy = Proxy
        DirectCast(request, System.Net.HttpWebRequest).KeepAlive = KeepAlive
        Return request
    End Function

    Protected Overrides Function GetWebResponse(request As WebRequest) As WebResponse
        Dim response As WebResponse = Nothing
        Try
            response = MyBase.GetWebResponse(request)
            ResponseUrl = response.ResponseUri.ToString
        Catch ex As Exception
        End Try
        Return response
    End Function


End Class
