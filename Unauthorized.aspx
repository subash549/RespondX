<%@ Page Title="Access Denied" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Unauthorized.aspx.cs" Inherits="RespondX.Unauthorized" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="status-page">
            <img runat="server" src="~/Content/Images/unauthorized.svg" alt="" class="status-illustration" width="200" height="150" />
            <h1>Access denied</h1>
            <p>Your account doesn't have permission to open this page. If you think this is a mistake, contact an administrator.</p>
            <div class="status-actions">
                <asp:HyperLink ID="lnkDashboard" runat="server" CssClass="btn btn-primary" Text="Go to My Dashboard" />
                <a runat="server" href="~/Logout.aspx" class="btn btn-secondary">Sign in as someone else</a>
            </div>
        </div>
    </div>
</asp:Content>
