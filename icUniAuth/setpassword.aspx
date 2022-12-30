<%@ Page Title="设置密码" Language="vb" AutoEventWireup="false" MasterPageFile="~/auth.master" CodeBehind="setpassword.aspx.vb" Inherits="icUniAuth.setpassword" %>

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
    <h2>设置密码</h2>
    <p>
        &nbsp;
    </p>
    <asp:TextBox ID="input_username" runat="server" placeholder="请输入邮箱"></asp:TextBox>
    <asp:TextBox ID="input_password" runat="server" TextMode="Password" placeholder="请输入密码"></asp:TextBox>
    <asp:TextBox ID="input_repassword" runat="server" TextMode="Password" placeholder="请再次输入密码"></asp:TextBox>
    <button id="btn_submit" class="btn" type="submit" runat="server" onserverclick="submit_click">提交</button>
    <script type="text/javascript">
        $('#bodys_btn_submit').click(function () {
            popup({
                type: 'load', msg: "正在提交请稍等", delay: null
            });
        })
    </script>
    <br />
</asp:Content>
