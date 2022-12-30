<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/portal/portal.Master" CodeBehind="authlog.aspx.vb" Inherits="icUniAuth.authlog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPH_B" runat="server">
    <div class="card-header">
        <h4>授权日志</h4>
        <div class="card-header-action">
            <asp:Button CssClass="btn btn-danger" ID="btn_clear" runat="server" Text="清除日志" />
        </div>
    </div>
    <div class="card-body">
        <div class="table-responsive table-invoice">
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>APP名称</th>
                        <th>授权时间</th>
                    </tr>
                </thead>
                <tbody>
                    <%
                        Dim logInfoList As List(Of icUniAuth.AccreditUtils.LogInfo) = icUniAuth.AccreditUtils.GetLogInfo(CType(Me.Master, icUniAuth.portal).UInfo)
                        For Each logInfo As icUniAuth.AccreditUtils.LogInfo In logInfoList
                            Response.Write("<tr><td>" + logInfo.AppName + "</td><td>" + logInfo.Time + "</td></tr>")
                        Next
                    %>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
