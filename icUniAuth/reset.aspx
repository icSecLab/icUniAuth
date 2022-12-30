<%@ Page Title="重置密码" Language="vb" AutoEventWireup="false" MasterPageFile="~/auth.master" CodeBehind="reset.aspx.vb" Inherits="icUniAuth.reset" %>
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
     <h2>重置密码</h2>
    <p>&nbsp;</p>
    <asp:TextBox ID="input_username" runat="server" placeholder="请输入邮箱"></asp:TextBox>
    <button id="btn_rst" class="btn" type="submit" runat="server" onserverclick="rst_click">重置</button>
    <script type="text/javascript">
        $('#bodys_btn_rst').click(function () {
            popup({
                type: 'load', msg: "正在提交请稍等", delay: null
            });
        })
    </script>

    <br />
    <p class="account">返回<a href="/login">登录</a> </p>
</asp:Content>
