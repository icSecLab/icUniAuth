<%@ Page Title="授权确认" Language="vb" AutoEventWireup="false" CodeBehind="authorize.aspx.vb" MasterPageFile="~/auth.master" Inherits="icUniAuth.authorize" %>

<asp:Content ID="Content1" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript" language="javascript">
        function Error(a) {
            popup({ type: 'error', msg: a, delay: 2000, width: 350, height: 160, callBack: function () { } });
        };
    </script>
    <script type="text/javascript" language="javascript">
        function SuccessRedirect(a, b) {
            popup({ type: 'success', msg: a, delay: 2000, width: 350, height: 160, callBack: function () { window.location.href = b } });
        };
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="bodys" runat="server">
    <h2>授权确认</h2>
    <p>&nbsp;</p>
    <p>
        <span id="bodys_user_name">当前用户:</span><asp:Label ID="user_name" runat="server" Style="color: black; font-weight: bold;"></asp:Label>
    </p>
    <p>
        <span id="bodys_app_name">请求APP:</span><asp:Label ID="app_name" runat="server" Style="color: black; font-weight: bold;"></asp:Label>
    </p>
    <p>
        &nbsp;
    </p>
    <p>
        请求授权内容:
    </p>
    <p style="margin-bottom: 20px;">
        <asp:Label ID="auth_info" runat="server" Style="color: black; font-weight: bold;"></asp:Label>
    </p>
    <table style="table-layout: fixed; width: 350px; align-content: center">
        <tr>
            <td>
                <button style="background: #63ed7a" id="btn_auth" class="btn" type="submit" runat="server" onserverclick="auth_click">同意授权</button></td>
            <td>
                <button style="background: #fc544b" id="btn_notauth" class="btn" type="submit" runat="server" onserverclick="notauth_click">拒绝授权</button></td>
        </tr>
    </table>

    <script type="text/javascript">
        $('#bodys_btn_notauth').click(function () {
            popup({
                type: 'load', msg: "正在提交请稍等", delay: null
            });
        })
    </script>
    <script type="text/javascript">
        $('#bodys_btn_auth').click(function () {
            popup({
                type: 'load', msg: "正在提交请稍等", delay: null
            });
        })
    </script>
    <br />
</asp:Content>
