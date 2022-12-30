<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/portal/portal.master" CodeBehind="authlist.aspx.vb" Inherits="icUniAuth.authlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPH_B" runat="server">
    <div class="card-header">
        <h4>授权列表</h4>
    </div>
    <div class="card-body">
        <div class="table-invoice">
            <table class="table" id="table-1">
                <thead>
                    <tr>
                        <th>APP名称</th>
                        <th>APPID</th>
                        <th>自动授权</th>
                        <th>操作</th>
                </thead>
                <tbody>
                    <%
                        'vb.net中+和&都可以用于连接字符串，但是更推荐&因为如果+忘记将其他类型转为string会报错，但是&会在连接之前将所有变量自动tostring
                        Dim authInfoList As List(Of icUniAuth.AccreditUtils.AuthInfo) = icUniAuth.AccreditUtils.GetAuthorization(CType(Me.Master, icUniAuth.portal).UInfo)
                        For Each authInfo As icUniAuth.AccreditUtils.AuthInfo In authInfoList
                            Response.Write("<tr><td>" & authInfo.AppName &
                                           "</td><td>" & authInfo.AppID &
                                           "</td><td>" & "<input disabled=""disabled"" type=""checkbox""  style=""margin-left: 0;"" " & If(authInfo.Auto, "checked=""checked""", "") & " />" & "<a href=""/portal/changeautoauth?appid=" & authInfo.AppID & """  >" & authInfo.Auto & "</a>" &
                                           "</td><td>" & "<a href=""/portal/revokeauth?appid=" & authInfo.AppID & """ class= ""btn btn-danger"" style=""color:#ffffff"">撤销授权</a>" &
                                           "</td></tr>")
                        Next
                    %>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
