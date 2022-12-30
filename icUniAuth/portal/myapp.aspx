<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/portal/portal.Master" CodeBehind="myapp.aspx.vb" Inherits="icUniAuth.myapp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPH_B" runat="server">
    <div class="card-header">
        <h4>我的APP</h4>

    </div>
    <div class="card-body">
        <div class="table-invoice">
            <table class="table" id="table-2">
                <thead>
                    <tr>
                        <th>APP名称</th>
                        <th>APPID</th>
                        <th>重定向地址</th>
                        <th>操作</th>
                </thead>
                <tbody>
                    <%
                        Response.Write(x)
                    %>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
