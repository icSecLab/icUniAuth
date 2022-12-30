<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/portal/portal.master" CodeBehind="reset.aspx.vb" Inherits="icUniAuth.reset1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPH_B" runat="server">
    <div class="card-header">
        <h4>密码重置</h4>
    </div>
    <div class="card-body">
        <div class="form-group">
            <asp:Label ID="lab_error" runat="server" ForeColor="Red" CssClass="form-group"></asp:Label>
        </div>

        <asp:Button CssClass="btn btn-warning" ID="btn_submit" runat="server" Text="发送重置密码邮件" />

    </div>

</asp:Content>
