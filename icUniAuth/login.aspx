<%@ Page Title="登录" Language="vb" AutoEventWireup="false" MasterPageFile="~/auth.master" CodeBehind="login.aspx.vb" Inherits="icUniAuth.login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript" language="javascript">
        function Error(a) {
            popup({ type: 'error', msg: a, delay: 2000, width: 350, height: 160, callBack: function () { } });
        };
    </script>
    <script type="text/javascript" language="javascript">
        function Success(a) {
            popup({ type: 'success', msg: a, delay: 2000, width: 350, height: 160, callBack: function () { window.location.href ="<%=redirect%>" } });
        };
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="bodys" runat="server">

    <h2>登录</h2>
    <p>&nbsp;</p>
    <asp:TextBox ID="input_username" runat="server" placeholder="请输入邮箱"></asp:TextBox>
    &nbsp;                     
    <asp:TextBox ID="input_password" runat="server" TextMode="Password" placeholder="请输入密码"></asp:TextBox>
    <p>&nbsp;</p>
    <button id="btn_login" class="btn" type="submit" runat="server" onserverclick="login_click">登录</button>
    <script type="text/javascript">
        $('#bodys_btn_login').click(function () {
            popup({
                type: 'load', msg: "正在登录请稍等", delay: null
            });
        })
    </script>

    <br />
    <p class="account">没有账号？<a href="/register">注册</a> &nbsp;&nbsp;&nbsp; 忘记密码？<a href="/reset">重置</a> </p>

</asp:Content>
