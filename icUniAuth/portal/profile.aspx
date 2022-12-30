<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/portal/portal.master" CodeBehind="profile.aspx.vb" Inherits="icUniAuth.profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPH_B" runat="server">
    <div class="card-header">
        <h4>个人信息</h4>
    </div>
    <div class="card-body">
        <div class="card-body">

            <asp:Label ID="lab_error" runat="server" ForeColor="Red" CssClass="form-group"></asp:Label>

            
            <div class="form-group" style="text-align: center;">
                  <asp:Image ID="img_profile" runat="server" CssClass="rounded-circle mr-1" Width="200px" Height="200px" />
            </div>

            <div class="form-group">
               <label >修改头像:</label>
                  <asp:FileUpload CssClass="btn-primary" ID="upload_profile_img" runat="server" />
            </div>

            <div class="form-group">
                <asp:Label  ID="lab_id" runat="server" Text="UID:" style="font-weight: 600;color: #34395e;font-size: 12px;letter-spacing: 0.5px;"></asp:Label>
            </div>

            <div class="form-group">
                <asp:Label ID="lab_email" runat="server" Text="邮箱:" style="font-weight: 600;color: #34395e;font-size: 12px;letter-spacing: 0.5px;"></asp:Label>
            </div>
            <div class="form-group">
                <label>昵称:</label>
                <asp:TextBox ID="input_nickname" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <asp:Button CssClass="btn btn-primary" ID="btn_submit" runat="server" Text="更新" Height="37px" Width="87px" />

        </div>
    </div>
</asp:Content>
