<%@ Page Title="Manage Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="RespondX.Admin.ManageUsers" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upUsers" runat="server">
        <ContentTemplate>
            <div class="container crud-region">
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

                <asp:Panel ID="pnlFilters" runat="server" CssClass="filter-bar" DefaultButton="btnSearch">
                    <div class="filter-group">
                        <label for="<%= ddlRole.ClientID %>">Role:</label>
                        <asp:DropDownList ID="ddlRole" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                            <asp:ListItem Value="All">All Roles</asp:ListItem>
                            <asp:ListItem Value="Learner">Learner</asp:ListItem>
                            <asp:ListItem Value="Expert">Expert</asp:ListItem>
                            <asp:ListItem Value="Admin">Admin</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="filter-group">
                        <label for="<%= ddlStatus.ClientID %>">Status:</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                            <asp:ListItem Value="All">All Status</asp:ListItem>
                            <asp:ListItem Value="Active">Active</asp:ListItem>
                            <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="filter-group">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search name, email or username" />
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="Filter_Changed" />
                    </div>
                    <div class="filter-group">
                        <asp:Button ID="btnAddUser" runat="server" Text="+ Add User" CssClass="btn btn-success" OnClick="btnAddUser_Click" />
                    </div>
                </asp:Panel>

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
                                <td>
                                    <strong><%#: Eval("FullName") %></strong>
                                    <span class="cell-sub">@<%#: Eval("Username") %></span>
                                </td>
                                <td><%#: Eval("Email") %></td>
                                <td><span class="badge badge-primary"><%#: Eval("Role") %></span></td>
                                <td><span class="user-status <%# (bool)Eval("IsActive") ? "active" : "inactive" %>"><%# (bool)Eval("IsActive") ? "Active" : "Inactive" %></span></td>
                                <td><%# Eval("CreatedAt", "{0:MMM dd, yyyy}") %></td>
                                <td>
                                    <div class="row-actions">
                                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="EditUser" CommandArgument='<%# Eval("UserID") %>' />
                                        <asp:Button ID="btnToggle" runat="server" Text='<%# (bool)Eval("IsActive") ? "Deactivate" : "Activate" %>' CssClass='<%# (bool)Eval("IsActive") ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="ToggleUser" CommandArgument='<%# Eval("UserID") %>' />
                                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="DeleteUser" CommandArgument='<%# Eval("UserID") %>' OnClientClick="return confirm('Delete this user permanently? This cannot be undone.');" />
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlNoUsers" runat="server" CssClass="empty-state" Visible="false">
                        <img runat="server" src="~/Content/Images/empty-state.svg" alt="" />
                        No users match these filters.
                    </asp:Panel>
                </div>

                <div class="modal fade" id="modalUser" tabindex="-1" role="dialog" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add User"></asp:Label></h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">&times;</button>
                            </div>
                            <asp:Panel ID="pnlUserForm" runat="server" CssClass="modal-body" DefaultButton="btnSaveUser">
                                <asp:HiddenField ID="hfUserID" runat="server" />
                                <asp:Panel ID="pnlModalError" runat="server" CssClass="alert alert-danger" Visible="false">
                                    <asp:Label ID="lblModalError" runat="server"></asp:Label>
                                </asp:Panel>
                                <div class="form-row">
                                    <div class="form-group">
                                        <label for="<%= txtFirstName.ClientID %>">First Name <span class="required">*</span></label>
                                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="50" />
                                    </div>
                                    <div class="form-group">
                                        <label for="<%= txtLastName.ClientID %>">Last Name <span class="required">*</span></label>
                                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="50" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="<%= txtEmail.ClientID %>">Email <span class="required">*</span></label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="100" />
                                </div>
                                <div class="form-row">
                                    <div class="form-group">
                                        <label for="<%= txtUsername.ClientID %>">Username <span class="required">*</span></label>
                                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" MaxLength="50" />
                                    </div>
                                    <div class="form-group">
                                        <label for="<%= ddlUserRole.ClientID %>">Role <span class="required">*</span></label>
                                        <asp:DropDownList ID="ddlUserRole" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="Learner">Learner</asp:ListItem>
                                            <asp:ListItem Value="Expert">Expert</asp:ListItem>
                                            <asp:ListItem Value="Admin">Admin</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="<%= txtPassword.ClientID %>">Password <asp:Label ID="lblPasswordRequired" runat="server" CssClass="required" Text="*" /></label>
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" autocomplete="new-password" />
                                    <asp:Label ID="lblPasswordHint" runat="server" CssClass="form-text" Text="At least 8 characters." />
                                </div>
                                <div class="form-group">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Text="Account is active" Checked="true" CssClass="form-check-inline" />
                                </div>
                            </asp:Panel>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnSaveUser" runat="server" Text="Save User" CssClass="btn btn-primary" OnClick="btnSaveUser_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
