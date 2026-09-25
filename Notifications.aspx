<%@ Page Title="Notifications - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="RespondX.Notifications" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="max-width:900px;margin:40px auto">
        <div class="page-header"><h1><i class="fas fa-bell"></i> Notifications</h1><p>Account and training updates for you.</p></div>
        <asp:Panel ID="pnlEmpty" runat="server" CssClass="card" Visible="false" style="padding:24px">You are all caught up.</asp:Panel>
        <asp:Repeater ID="rptNotifications" runat="server" OnItemCommand="rptNotifications_ItemCommand">
            <ItemTemplate>
                <div class="card" style="padding:18px 22px;margin-bottom:12px;border-left:4px solid <%# Convert.ToBoolean(Eval("IsRead")) ? "#cbd5e0" : "#2456c8" %>">
                    <div style="display:flex;justify-content:space-between;gap:20px">
                        <div><strong><%#: Eval("Title") %></strong><p style="margin:5px 0"><%#: Eval("Message") %></p><small><%#: Eval("TimeAgo") %></small></div>
                        <asp:LinkButton ID="btnOpen" runat="server" CommandName="Open" CommandArgument='<%# Eval("NotificationID") %>' Text="View" CssClass="btn btn-secondary btn-sm" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
