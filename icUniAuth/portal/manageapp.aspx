<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/portal/portal.master" CodeBehind="manageapp.aspx.vb" Inherits="icUniAuth.manageapp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPH_B" runat="server">
    <div class="card-header">
        <h4>APP管理</h4>
    </div>
    <div class="card-body">

        <asp:Label ID="lab_error" runat="server" ForeColor="Red" CssClass="form-group"></asp:Label>


        <br />


        <div class="form-group">
            <label>
                APP ID<br />
            </label>
            <br />
            <asp:Label ID="lab_appid" runat="server" CssClass="form-group"></asp:Label>
        </div>

        <div class="form-group">
            <label>
                APP Secret<br />
            </label>
            <br />
            <asp:Label ID="lab_secret" runat="server" CssClass="form-group"></asp:Label>
            <asp:Button CssClass="btn btn-primary" ID="reset" runat="server" Text="重置" />
        </div>

        <div class="form-group">
            <label>APP名称</label>
            <br />
            <asp:TextBox ID="input_appname" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="form-group">
            <label>重定向地址</label>
            <br />
            <asp:TextBox ID="input_redirect" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="form-group">
            <label>操作</label>
            <br />
            <asp:Button CssClass="btn btn-success" ID="btn_submit" runat="server" Text="提交" />
            <asp:Button CssClass="btn btn-danger" ID="btn_delete" runat="server" Text="删除" />
            <asp:Button CssClass="btn btn-primary" ID="btn_exit" runat="server" Text="退出" />
        </div>
    </div>
</asp:Content>
