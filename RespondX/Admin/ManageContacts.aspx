<%@ Page Title="Manage Emergency Contacts - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageContacts.aspx.cs" Inherits="RespondX.Admin.ManageContacts" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Admin.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-address-book"></i> Manage Emergency Contacts</h1>
            <p>View and manage emergency contacts for all learners</p>
        </div>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Learner:</label>
                <asp:DropDownList ID="ddlLearner" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLearner_SelectedIndexChanged">
                    <asp:ListItem Value="All">All Learners</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search contacts..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
        </div>

        <div class="table-responsive">
            <asp:Repeater ID="rptContacts" runat="server" OnItemCommand="rptContacts_ItemCommand">
                <HeaderTemplate>
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th>Learner</th>
                                <th>Contact Name</th>
                                <th>Relationship</th>
                                <th>Phone</th>
                                <th>Email</th>
                                <th>Primary</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("LearnerName") %></td>
                        <td><strong><%# Eval("FullName") %></strong></td>
                        <td><%# Eval("Relationship") %></td>
                        <td><%# Eval("PhoneNumber") %></td>
                        <td><%# Eval("Email") %></td>
                        <td><%# Convert.ToBoolean(Eval("IsPrimary")) ? "<span class='badge badge-success'>Primary</span>" : "" %></td>
                        <td>
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("ContactID") %>' />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="Delete" CommandArgument='<%# Eval("ContactID") %>' OnClientClick="return confirm('Are you sure you want to delete this contact?');" />
                            <asp:Button ID="btnSetPrimary" runat="server" Text="Set Primary" CssClass="btn btn-success btn-sm" CommandName="SetPrimary" CommandArgument='<%# Eval("ContactID") %>' Visible='<%# !Convert.ToBoolean(Eval("IsPrimary")) %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                    <asp:Label ID="lblNoContacts" runat="server" Text="No contacts found" Visible='<%# rptContacts.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>

    <style>
        .page-header {
            margin-bottom: 30px;
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
        @media (max-width: 768px) {
            .filter-bar {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>