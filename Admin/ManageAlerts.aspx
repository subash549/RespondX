<%@ Page Title="Manage Alerts - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageAlerts.aspx.cs" Inherits="RespondX.Admin.ManageAlerts" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Admin.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-bell"></i> Manage Alerts</h1>
            <p>Create and manage system alerts and notifications</p>
        </div>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Type:</label>
                <asp:DropDownList ID="ddlType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                    <asp:ListItem Value="All">All Types</asp:ListItem>
                    <asp:ListItem Value="Emergency">Emergency</asp:ListItem>
                    <asp:ListItem Value="Reminder">Reminder</asp:ListItem>
                    <asp:ListItem Value="Notification">Notification</asp:ListItem>
                    <asp:ListItem Value="System">System</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <label>Status:</label>
                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="Active">Active</asp:ListItem>
                    <asp:ListItem Value="Expired">Expired</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <asp:Button ID="btnAddAlert" runat="server" Text="+ Create Alert" CssClass="btn btn-success" OnClick="btnAddAlert_Click" />
            </div>
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
                                <span><i class="fas fa-user"></i> To: <%# Eval("Target") %></span>
                                <span><i class="fas fa-clock"></i> <%# Eval("CreatedAt", "{0:MMM dd, yyyy HH:mm}") %></span>
                                <span class="alert-priority">Priority: <%# Eval("PriorityLevel") %></span>
                                <span class="badge <%# Convert.ToBoolean(Eval("IsActive")) ? "badge-success" : "badge-secondary" %>">
                                    <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Expired" %>
                                </span>
                            </div>
                        </div>
                        <div class="alert-actions">
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("AlertID") %>' />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="Delete" CommandArgument='<%# Eval("AlertID") %>' OnClientClick="return confirm('Are you sure you want to delete this alert?');" />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoAlerts" runat="server" Text="No alerts found" Visible='<%# rptAlerts.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="modal fade" id="modalAlert" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Create Alert"></asp:Label></h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfAlertID" runat="server" />
                        <div class="form-group">
                            <label>Title *</label>
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <label>Message *</label>
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                        </div>
                        <div class="form-group">
                            <label>Alert Type</label>
                            <asp:DropDownList ID="ddlAlertType" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Emergency">Emergency</asp:ListItem>
                                <asp:ListItem Value="Reminder">Reminder</asp:ListItem>
                                <asp:ListItem Value="Notification">Notification</asp:ListItem>
                                <asp:ListItem Value="System">System</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>Priority</label>
                            <asp:DropDownList ID="ddlPriority" runat="server" CssClass="form-control">
                                <asp:ListItem Value="1">Very Low</asp:ListItem>
                                <asp:ListItem Value="2">Low</asp:ListItem>
                                <asp:ListItem Value="3">Medium</asp:ListItem>
                                <asp:ListItem Value="4">High</asp:ListItem>
                                <asp:ListItem Value="5">Critical</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>Target Audience</label>
                            <asp:DropDownList ID="ddlTarget" runat="server" CssClass="form-control">
                                <asp:ListItem Value="All">All Users</asp:ListItem>
                                <asp:ListItem Value="Learners">Learners Only</asp:ListItem>
                                <asp:ListItem Value="Experts">Experts Only</asp:ListItem>
                                <asp:ListItem Value="Admins">Admins Only</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>Expires After</label>
                            <asp:DropDownList ID="ddlExpires" runat="server" CssClass="form-control">
                                <asp:ListItem Value="0">Never</asp:ListItem>
                                <asp:ListItem Value="1">1 Day</asp:ListItem>
                                <asp:ListItem Value="7">7 Days</asp:ListItem>
                                <asp:ListItem Value="30">30 Days</asp:ListItem>
                                <asp:ListItem Value="90">90 Days</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnSaveAlert" runat="server" Text="Create" CssClass="btn btn-primary" OnClick="btnSaveAlert_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <style>
        .page-header {
            margin-bottom: 30px;
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
        .alert-item.critical { border-left-color: #fc8181; background: #fff5f5; }
        .alert-item.high { border-left-color: #fc8181; }
        .alert-item.medium { border-left-color: #f6ad55; }
        .alert-item.low { border-left-color: #63b3ed; }
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
        }
        .alert-footer {
            display: flex;
            gap: 15px;
            font-size: 13px;
            color: #718096;
            flex-wrap: wrap;
        }
        .alert-actions {
            display: flex;
            gap: 8px;
            flex-shrink: 0;
        }
        .filter-bar {
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 20px;
            background: white;
            padding: 15px 20px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .filter-group {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
        }
        .filter-group label {
            margin: 0;
            font-weight: 600;
        }
        .filter-group select, .filter-group input {
            padding: 8px 12px;
            border: 2px solid #e2e8f0;
            border-radius: 5px;
        }
        @media (max-width: 768px) {
            .alert-item {
                flex-direction: column;
            }
            .alert-actions {
                width: 100%;
                justify-content: flex-start;
            }
            .filter-bar {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>