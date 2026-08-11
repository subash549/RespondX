<%@ Page Title="Manage Users - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="RespondX.Admin.ManageUsers" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Admin.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-users-cog"></i> Manage Users</h1>
            <p>View, edit, and manage user accounts</p>
        </div>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Role:</label>
                <asp:DropDownList ID="ddlRole" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged">
                    <asp:ListItem Value="All">All Roles</asp:ListItem>
                    <asp:ListItem Value="Learner">Learner</asp:ListItem>
                    <asp:ListItem Value="Expert">Expert</asp:ListItem>
                    <asp:ListItem Value="Admin">Admin</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <label>Status:</label>
                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                    <asp:ListItem Value="All">All Status</asp:ListItem>
                    <asp:ListItem Value="Active">Active</asp:ListItem>
                    <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search users..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
            <div class="filter-group">
                <asp:Button ID="btnAddUser" runat="server" Text="+ Add User" CssClass="btn btn-success" OnClick="btnAddUser_Click" />
            </div>
        </div>

        <div class="table-responsive">
            <asp:Repeater ID="rptUsers" runat="server" OnItemCommand="rptUsers_ItemCommand">
                <HeaderTemplate>
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th>User</th>
                                <th>Email</th>
                                <th>Role</th>
                                <th>Status</th>
                                <th>Joined</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><strong><%# Eval("FullName") %></strong></td>
                        <td><%# Eval("Email") %></td>
                        <td><span class="badge badge-primary"><%# Eval("Role") %></span></td>
                        <td><span class="user-status <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "inactive" %>"><%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %></span></td>
                        <td><%# Eval("CreatedAt", "{0:MMM dd, yyyy}") %></td>
                        <td>
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("UserID") %>' />
                            <asp:Button ID="btnToggle" runat="server" Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="Toggle" CommandArgument='<%# Eval("UserID") %>' />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="Delete" CommandArgument='<%# Eval("UserID") %>' OnClientClick="return confirm('Are you sure you want to delete this user?');" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                    <asp:Label ID="lblNoUsers" runat="server" Text="No users found" Visible='<%# rptUsers.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="modal fade" id="modalUser" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add User"></asp:Label></h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfUserID" runat="server" />
                        <div class="form-group">
                            <label>First Name *</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <label>Last Name *</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <label>Email *</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                        </div>
                        <div class="form-group">
                            <label>Username *</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <label>Role *</label>
                            <asp:DropDownList ID="ddlUserRole" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Learner">Learner</asp:ListItem>
                                <asp:ListItem Value="Expert">Expert</asp:ListItem>
                                <asp:ListItem Value="Admin">Admin</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group" id="passwordGroup">
                            <label>Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
                            <small class="form-text text-muted">Leave blank to keep current password (when editing)</small>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnSaveUser" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveUser_Click" />
                    </div>
                </div>
            </div>
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
        .admin-table {
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .admin-table thead {
            background: #f7fafc;
        }
        .admin-table th {
            padding: 12px 15px;
            font-weight: 600;
            color: #2d3748;
        }
        .admin-table td {
            padding: 12px 15px;
            vertical-align: middle;
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
        .user-status {
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            display: inline-block;
        }
        .user-status.active {
            background: #c6f6d5;
            color: #22543d;
        }
        .user-status.inactive {
            background: #fed7d7;
            color: #9b2c2c;
        }
        .text-center {
            text-align: center;
        }
        .d-block {
            display: block;
        }
        .form-text {
            font-size: 12px;
            color: #718096;
            margin-top: 3px;
        }
        @media (max-width: 768px) {
            .filter-bar {
                flex-direction: column;
            }
            .filter-group {
                width: 100%;
            }
            .filter-group select, .filter-group input {
                flex: 1;
            }
        }
    </style>
</asp:Content>