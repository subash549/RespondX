<%@ Page Title="Manage Categories" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageCategories.aspx.cs" Inherits="RespondX.Admin.ManageCategories" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upCategories" runat="server">
        <ContentTemplate>
            <div class="container crud-region">
                <div class="page-header">
                    <h1><i class="fas fa-tags"></i> Manage Categories</h1>
                    <p>Create, edit, and organize course categories</p>
                </div>

                <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                    <asp:Label ID="lblSuccess" runat="server"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="pnlFilters" runat="server" CssClass="filter-bar" DefaultButton="btnSearch">
                    <div class="filter-group">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search categories" />
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                    </div>
                    <div class="filter-group">
                        <asp:Button ID="btnAddCategory" runat="server" Text="+ Add Category" CssClass="btn btn-success" OnClick="btnAddCategory_Click" UseSubmitBehavior="false" />
                    </div>
                </asp:Panel>

                <div class="crud-grid">
                    <asp:Repeater ID="rptCategories" runat="server" OnItemCommand="rptCategories_ItemCommand">
                        <ItemTemplate>
                            <div class="crud-card <%# (bool)Eval("IsActive") ? "" : "is-inactive" %>">
                                <div class="crud-card-header">
                                    <h3><%#: Eval("Name") %></h3>
                                    <span class="badge <%# (bool)Eval("IsActive") ? "badge-success" : "badge-secondary" %>"><%# (bool)Eval("IsActive") ? "Active" : "Inactive" %></span>
                                </div>
                                <p class="crud-card-body"><%#: Eval("Description") %></p>
                                <div class="crud-card-meta">
                                    <span><i class="fas fa-book"></i> <%# Eval("ModuleCount") %> module(s)</span>
                                </div>
                                <div class="crud-card-actions">
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="EditCategory" CommandArgument='<%# Eval("CategoryID") %>' />
                                    <asp:Button ID="btnToggle" runat="server" Text='<%# (bool)Eval("IsActive") ? "Deactivate" : "Activate" %>' CssClass='<%# (bool)Eval("IsActive") ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="ToggleCategory" CommandArgument='<%# Eval("CategoryID") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="DeleteCategory" CommandArgument='<%# Eval("CategoryID") %>' OnClientClick="return confirm('Delete this category?');" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlNoCategories" runat="server" CssClass="empty-state" Visible="false">
                        <img runat="server" src="~/Content/Images/empty-state.svg" alt="" />
                        No categories found.
                    </asp:Panel>
                </div>

                <div class="modal fade" id="modalCategory" tabindex="-1" role="dialog" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Category"></asp:Label></h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">&times;</button>
                            </div>
                            <asp:Panel ID="pnlCategoryForm" runat="server" CssClass="modal-body" DefaultButton="btnSaveCategory">
                                <asp:HiddenField ID="hfCategoryID" runat="server" />
                                <asp:Panel ID="pnlModalError" runat="server" CssClass="alert alert-danger" Visible="false">
                                    <asp:Label ID="lblModalError" runat="server"></asp:Label>
                                </asp:Panel>
                                <div class="form-group">
                                    <label for="<%= txtName.ClientID %>">Name <span class="required">*</span></label>
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="100" />
                                </div>
                                <div class="form-group">
                                    <label for="<%= txtDescription.ClientID %>">Description</label>
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="500" />
                                </div>
                                <div class="form-group">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" Text="Active" CssClass="form-check-inline" />
                                </div>
                            </asp:Panel>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnSaveCategory" runat="server" Text="Save Category" CssClass="btn btn-primary" OnClick="btnSaveCategory_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
