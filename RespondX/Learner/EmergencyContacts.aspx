<%@ Page Title="Emergency Contacts - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmergencyContacts.aspx.cs" Inherits="RespondX.Learner.EmergencyContacts" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-address-book"></i> Emergency Contacts</h1>
            <p>Manage your emergency contact information</p>
        </div>

        <div class="card">
            <div class="card-title">
                <span>Your Contacts</span>
                <button type="button" class="btn btn-primary btn-sm" data-toggle="modal" data-target="#modalAddContact">
                    <i class="fas fa-plus"></i> Add Contact
                </button>
            </div>
            <div class="table-responsive">
                <asp:Repeater ID="rptContacts" runat="server" OnItemCommand="rptContacts_ItemCommand">
                    <HeaderTemplate>
                        <table class="table">
                            <thead>
                                <tr>
                                    <th>Name</th>
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
                        <asp:Label ID="lblNoContacts" runat="server" Text="No emergency contacts added yet." Visible='<%# rptContacts.Items.Count == 0 %>' CssClass="text-muted" />
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>

    <!-- Add Contact Modal -->
    <div class="modal fade" id="modalAddContact" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Emergency Contact</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label>Full Name *</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" CssClass="validation-error" ErrorMessage="Name is required" />
                    </div>
                    <div class="form-group">
                        <label>Relationship *</label>
                        <asp:TextBox ID="txtRelationship" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvRelationship" runat="server" ControlToValidate="txtRelationship" CssClass="validation-error" ErrorMessage="Relationship is required" />
                    </div>
                    <div class="form-group">
                        <label>Phone Number *</label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone" CssClass="validation-error" ErrorMessage="Phone number is required" />
                    </div>
                    <div class="form-group">
                        <label>Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$" CssClass="validation-error" ErrorMessage="Invalid email address" />
                    </div>
                    <div class="form-group">
                        <asp:CheckBox ID="chkPrimary" runat="server" />
                        <label for="chkPrimary">Set as primary contact</label>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnSave" runat="server" Text="Save Contact" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                </div>
            </div>
        </div>
    </div>

    <style>
        .validation-error {
            color: #fc8181;
            font-size: 12px;
            display: block;
            margin-top: 3px;
        }
        .modal-header .close {
            padding: 0;
            background: none;
            border: none;
            font-size: 28px;
            line-height: 1;
        }
        .modal-body {
            padding: 20px;
        }
    </style>
</asp:Content>