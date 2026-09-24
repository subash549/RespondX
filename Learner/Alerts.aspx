<%@ Page Title="Alerts - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Alerts.aspx.cs" Inherits="RespondX.Learner.Alerts" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-bell"></i> Alerts & Notifications</h1>
            <p>Stay informed about important updates and reminders</p>
        </div>

        <div class="alert-controls">
            <asp:Button ID="btnMarkAllRead" runat="server" Text="Mark All as Read" CssClass="btn btn-secondary" OnClick="btnMarkAllRead_Click" />
            <asp:Button ID="btnClearAll" runat="server" Text="Clear All" CssClass="btn btn-danger" OnClick="btnClearAll_Click" OnClientClick="return confirm('Are you sure you want to clear all alerts?');" />
        </div>

        <div class="alerts-list">
            <asp:Repeater ID="rptAlerts" runat="server" OnItemCommand="rptAlerts_ItemCommand">
                <ItemTemplate>
                    <div class="alert-item <%# Eval("PriorityClass") %>" id="alert_<%# Eval("AlertID") %>">
                        <div class="alert-content">
                            <div class="alert-header">
                                <h4><%# Eval("Title") %></h4>
                                <span class="badge <%# Eval("TypeBadge") %>"><%# Eval("AlertTypeDisplay") %></span>
                            </div>
                            <p><%# Eval("Message") %></p>
                            <div class="alert-footer">
                                <span class="alert-time"><i class="fas fa-clock"></i> <%# Eval("TimeAgo") %></span>
                                <span class="alert-priority">Priority: <%# Eval("PriorityLevel") %></span>
                            </div>
                        </div>
                        <div class="alert-actions">
                            <%# Convert.ToBoolean(Eval("IsRead")) ? "<span class='badge badge-secondary'>Read</span>" : "<span class='badge badge-primary'>Unread</span>" %>
                            <asp:Button ID="btnAcknowledge" runat="server" Text="Acknowledge" CssClass="btn btn-success btn-sm" CommandName="Acknowledge" CommandArgument='<%# Eval("AlertID") %>' Visible='<%# !Convert.ToBoolean(Eval("IsAcknowledged")) %>' />
                            <asp:Button ID="btnDismiss" runat="server" Text="Dismiss" CssClass="btn btn-secondary btn-sm" CommandName="Dismiss" CommandArgument='<%# Eval("AlertID") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Panel ID="pnlNoAlerts" runat="server" Visible='<%# rptAlerts.Items.Count == 0 %>'>
                        <div class="no-alerts">
                            <i class="fas fa-bell fa-4x text-muted"></i>
                            <h3>No Alerts</h3>
                            <p>You're all caught up! No new alerts or notifications.</p>
                        </div>
                    </asp:Panel>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>

    <style>
        .page-header {
            margin-bottom: 30px;
        }
        .page-header h1 {
            margin-bottom: 5px;
        }
        .page-header p {
            color: #718096;
        }
        .alert-controls {
            display: flex;
            gap: 10px;
            margin-bottom: 20px;
            justify-content: flex-end;
        }
        .alerts-list {
            display: grid;
            gap: 15px;
        }
        .alert-item {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 20px;
            border-left: 4px solid #667eea;
        }
        .alert-item.critical {
            border-left-color: #fc8181;
            background: #fff5f5;
        }
        .alert-item.high {
            border-left-color: #fc8181;
        }
        .alert-item.medium {
            border-left-color: #f6ad55;
        }
        .alert-item.low {
            border-left-color: #63b3ed;
        }
        .alert-content {
            flex: 1;
        }
        .alert-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 5px;
        }
        .alert-header h4 {
            margin: 0;
            color: #2d3748;
        }
        .alert-content p {
            margin: 10px 0;
            color: #4a5568;
            line-height: 1.6;
        }
        .alert-footer {
            display: flex;
            gap: 20px;
            font-size: 13px;
            color: #718096;
            flex-wrap: wrap;
        }
        .alert-footer i {
            margin-right: 3px;
        }
        .alert-actions {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            align-items: center;
            flex-shrink: 0;
        }
        .no-alerts {
            text-align: center;
            padding: 60px 20px;
            background: white;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .no-alerts h3 {
            margin: 20px 0 10px;
            color: #2d3748;
        }
        .no-alerts p {
            color: #718096;
        }
        @media (max-width: 768px) {
            .alert-item {
                flex-direction: column;
            }
            .alert-actions {
                width: 100%;
                justify-content: flex-start;
            }
            .alert-controls {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>