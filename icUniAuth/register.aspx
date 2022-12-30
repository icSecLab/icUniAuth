<%@ Page Title="注册" Language="vb" AutoEventWireup="false" MasterPageFile="~/auth.master" CodeBehind="register.aspx.vb" Inherits="icUniAuth.register" %>


<asp:Content ID="Content1" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript" language="javascript">
        function Error(a) {
            popup({ type: 'error', msg: a, delay: 2000, width: 350, height: 160, callBack: function () { } });
        };
    </script>
    <script type="text/javascript" language="javascript">
        function Success(a, b) {
            popup({ type: 'success', msg: a, delay: 2000, width: 350, height: 160, callBack: function () { window.location.href = b } });
        };
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="bodys" runat="server">

    <h2>注册</h2>
    <p>&nbsp;</p>
    <asp:TextBox ID="input_username" runat="server" placeholder="请输入邮箱"></asp:TextBox>
    <asp:TextBox ID="input_password" runat="server" TextMode="Password" placeholder="请输入密码"></asp:TextBox>
    <asp:TextBox ID="input_repassword" runat="server" TextMode="Password" placeholder="请再次输入密码"></asp:TextBox>
    <button id="btn_reg" class="btn" type="submit" runat="server" onserverclick="reg_click">注册</button>
    <script type="text/javascript">
        $('#bodys_btn_reg').click(function () {
            popup({
                type: 'load', msg: "正在提交请稍等", delay: null
            });
        })
    </script>

    <br />
    <p class="account">已有账号？<a href="/login">登录</a> </p>

</asp:Content>
