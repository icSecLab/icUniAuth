<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/portal/portal.master" CodeBehind="newapp.aspx.vb" Inherits="icUniAuth.newapp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPH_B" runat="server">
    <div class="card-header">
        <h4>APP接入申请</h4>
    </div>
    <div class="card-body">
        <div class="card-body">
               
            <asp:Label ID="lab_error" runat="server" ForeColor="Red" CssClass ="form-group"></asp:Label>
            <div class="form-group">
                <label>APP名称</label>
                <asp:TextBox ID="input_appname" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>重定向地址</label>
                <asp:TextBox ID="redirect" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            
            <asp:Button CssClass="btn btn-primary" ID="btn_submit" runat="server" Text="提交" Height="37px" Width="87px" />
            
        </div>
    </div>

</asp:Content>
