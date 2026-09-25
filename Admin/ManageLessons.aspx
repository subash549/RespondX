<%@ Page Title="Manage Lessons - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageLessons.aspx.cs" Inherits="RespondX.Admin.ManageLessons" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upLessons" runat="server">
        <ContentTemplate>
            <div class="container crud-region">
                <div class="page-header">
                    <h1><i class="fas fa-list-ul"></i> Manage Lessons</h1>
                    <p><asp:Label ID="lblModuleInfo" runat="server"></asp:Label></p>
                </div>

                <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                    <asp:Label ID="lblSuccess" runat="server"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </asp:Panel>

                <div class="filter-bar">
                    <div class="filter-group">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-secondary" OnClick="btnBack_Click"><img runat="server" src="~/Content/Images/icons/arrow-left.svg" alt="" class="img-icon" />Back to Modules</asp:LinkButton>
                    </div>
                    <div class="filter-group">
                        <asp:Button ID="btnAddLesson" runat="server" Text="+ Add Lesson" CssClass="btn btn-success" OnClick="btnAddLesson_Click" CausesValidation="false" />
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:Repeater ID="rptLessons" runat="server" OnItemCommand="rptLessons_ItemCommand">
                        <HeaderTemplate>
                            <table class="table admin-table">
                                <thead>
                                    <tr>
                                        <th>Order</th>
                                        <th>Title</th>
                                        <th>Status</th>
                                        <th>Video</th>
                                        <th>Resources</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("LessonOrder") %></td>
                                <td><strong><%# Server.HtmlEncode(Convert.ToString(Eval("Title"))) %></strong></td>
                                <td><span class="badge <%# Convert.ToBoolean(Eval("IsActive")) ? "badge-success" : "badge-secondary" %>"><%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %></span></td>
                                <td><%# Convert.ToBoolean(Eval("HasVideo")) ? "<img src='" + ResolveUrl("~/Content/Images/icons/check.svg") + "' class='img-icon' alt='Yes' />" : "<img src='" + ResolveUrl("~/Content/Images/icons/cross.svg") + "' class='img-icon' alt='No' />" %></td>
                                <td><%# Convert.ToBoolean(Eval("HasResources")) ? "<img src='" + ResolveUrl("~/Content/Images/icons/check.svg") + "' class='img-icon' alt='Yes' />" : "<img src='" + ResolveUrl("~/Content/Images/icons/cross.svg") + "' class='img-icon' alt='No' />" %></td>
                                <td>
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("LessonID") %>' />
                                    <asp:Button ID="btnToggle" runat="server" Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="Toggle" CommandArgument='<%# Eval("LessonID") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="Delete" CommandArgument='<%# Eval("LessonID") %>' OnClientClick="return confirm('Delete this lesson? Its learner progress will also be removed.');" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                            <asp:Label ID="lblNoLessons" runat="server" Text="No lessons found" Visible='<%# rptLessons.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <asp:Panel ID="pnlLessonEditor" runat="server" CssClass="card lesson-editor" Visible="false">
                    <div class="lesson-editor-heading">
                        <div><h2><asp:Label ID="lblModalTitle" runat="server" Text="Add Lesson" /></h2><p>Enter the material learners need to read or watch before completing this lesson.</p></div>
                        <asp:Button ID="btnCancelLesson" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancelLesson_Click" CausesValidation="false" />
                    </div>
                    <asp:HiddenField ID="hfLessonID" runat="server" />
                    <asp:HiddenField ID="hfModuleID" runat="server" />
                    <asp:Panel ID="pnlModalError" runat="server" CssClass="alert alert-danger" Visible="false"><asp:Label ID="lblModalError" runat="server" /></asp:Panel>
                    <div class="form-group"><label for="<%= txtTitle.ClientID %>">Lesson Title *</label><asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="100" /></div>
                    <div class="form-group"><label for="<%= txtContent.ClientID %>">Lesson Content *</label><asp:TextBox ID="txtContent" runat="server" CssClass="form-control lesson-content-input" TextMode="MultiLine" Rows="8" /><small>Write instructions, an explanation, or an activity. Learners can mark the lesson complete once it is available.</small></div>
                    <div class="form-row">
                        <div class="form-group col-6"><label for="<%= txtOrder.ClientID %>">Lesson Order</label><asp:TextBox ID="txtOrder" runat="server" CssClass="form-control" TextMode="Number" placeholder="Auto" /></div>
                        <div class="form-group col-6"><label for="<%= txtVideoUrl.ClientID %>">Video URL (optional)</label><asp:TextBox ID="txtVideoUrl" runat="server" CssClass="form-control" placeholder="https://..." /></div>
                    </div>
                    <div class="form-group"><label for="<%= txtResourceUrl.ClientID %>">Resource URL (optional)</label><asp:TextBox ID="txtResourceUrl" runat="server" CssClass="form-control" placeholder="https://..." /></div>
                    <div class="lesson-editor-footer"><asp:CheckBox ID="chkIsActive" runat="server" Checked="true" Text="Active and visible to learners" /><asp:Button ID="btnSaveLesson" runat="server" Text="Save Lesson" CssClass="btn btn-primary" OnClick="btnSaveLesson_Click" /></div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

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
        .lesson-editor { padding: 24px; margin-bottom: 22px; }
        .lesson-editor-heading, .lesson-editor-footer { display:flex; align-items:center; justify-content:space-between; gap:16px; margin-bottom:20px; }
        .lesson-editor-heading h2 { margin:0 0 4px; font-size:20px; }
        .lesson-editor-heading p, .lesson-editor small { color:#718096; margin:0; }
        .lesson-editor .form-group { margin-bottom:16px; }
        .lesson-content-input { min-height:180px; }
        .lesson-editor-footer { margin:0; padding-top:16px; border-top:1px solid #e2e8f0; }
        .text-center {
            text-align: center;
        }
        .d-block {
            display: block;
        }
        .form-row {
            display: flex;
            gap: 15px;
        }
        .form-row .form-group {
            flex: 1;
        }
        @media (max-width: 768px) {
            .filter-bar {
                flex-direction: column;
            }
            .form-row {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>
