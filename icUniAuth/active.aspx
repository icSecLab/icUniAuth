<%@ Page Title="激活" Language="vb" AutoEventWireup="false" MasterPageFile="~/auth.master" CodeBehind="active.aspx.vb" Inherits="icUniAuth.active" %>

<asp:Content ID="Content1" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript" language="javascript">
        function ErrorRedirect(a) {
            popup({ type: 'error', msg: a, delay: 2000, width: 350, height: 160, callBack: function () { window.location.href ="<%=redirect%>" } });
        };
    </script>

    <script type="text/javascript" language="javascript">
        function Error(a) {
            popup({ type: 'error', msg: a, delay: 2000, width: 350, height: 160 });
        };
    </script>
    <script type="text/javascript" language="javascript">
        function SuccessRedirect(a) {
            popup({ type: 'success', msg: a, delay: 2000, width: 350, height: 160, callBack: function () { window.location.href = "<%=redirect%>" } });
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodys" runat="server">
    <h2>激活</h2>


    <p>
        &nbsp;
    </p>
    <asp:TextBox ID="input_username" runat="server" placeholder="请输入注册邮箱"></asp:TextBox>
    <asp:TextBox ID="input_nickname" runat="server" placeholder="请输入昵称"></asp:TextBox>
    <button id="btn_active" class="btn" type="submit" runat="server" onserverclick="active_click">激活</button>
    <script type="text/javascript">
        $('#bodys_btn_active').click(function () {
            popup({
                type: 'load', msg: "正在提交请稍等", delay: null
            });
        })
    </script>

    <br />
</asp:Content>
