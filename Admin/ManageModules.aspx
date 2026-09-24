<%@ Page Title="Manage Modules - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageModules.aspx.cs" Inherits="RespondX.Admin.ManageModules" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Admin.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-book"></i> Manage Modules</h1>
            <p>Create, edit, and organize learning modules</p>
        </div>

        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
            <asp:Label ID="lblSuccess" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
            <asp:Label ID="lblError" runat="server"></asp:Label>
        </asp:Panel>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Status:</label>
                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="Active">Active</asp:ListItem>
                    <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search modules..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
            <div class="filter-group">
                <asp:Button ID="btnAddModule" runat="server" Text="+ Add Module" CssClass="btn btn-success" OnClick="btnAddModule_Click" />
            </div>
        </div>

        <div class="modules-grid">
            <asp:Repeater ID="rptModules" runat="server" OnItemCommand="rptModules_ItemCommand">
                <ItemTemplate>
                    <div class="module-card admin-module-card">
                        <div class="module-header">
                            <h3><%# Eval("Title") %></h3>
                            <span class="badge <%# Convert.ToBoolean(Eval("IsActive")) ? "badge-success" : "badge-secondary" %>">
                                <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                            </span>
                        </div>
                        <p class="module-description"><%# Eval("Description") %></p>
                        <div class="module-meta">
                            <span><i class="fas fa-list"></i> <%# Eval("LessonCount") %> Lessons</span>
                            <span><i class="fas fa-clock"></i> <%# Eval("EstimatedHours") %> hours</span>
                            <span><i class="fas fa-calendar"></i> <%# Eval("CreatedAt", "{0:MMM dd, yyyy}") %></span>
                        </div>
                        <div class="module-actions">
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("ModuleID") %>' />
                            <asp:Button ID="btnLessons" runat="server" Text="Lessons" CssClass="btn btn-primary btn-sm" CommandName="Lessons" CommandArgument='<%# Eval("ModuleID") %>' />
                            <asp:Button ID="btnToggle" runat="server" Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="Toggle" CommandArgument='<%# Eval("ModuleID") %>' />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="Delete" CommandArgument='<%# Eval("ModuleID") %>' OnClientClick="return confirm('Are you sure you want to delete this module?');" />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoModules" runat="server" Text="No modules found" Visible='<%# rptModules.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="modal fade" id="modalModule" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Module"></asp:Label></h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfModuleID" runat="server" />
                        <div class="form-group">
                            <label>Title *</label>
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <label>Module Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"
                                placeholder="Describe what learners will learn in this module..." />
                        </div>
                        <div class="form-group">
                            <label>YouTube Video URL</label>
                            <asp:TextBox ID="txtYouTubeVideoUrl" runat="server" CssClass="form-control"
                                placeholder="https://www.youtube.com/watch?v=..." />
                            <small class="text-muted">Paste a YouTube link to show on the learner module page.</small>
                        </div>
                        <div class="form-group">
                            <label>Upload Module Video</label>
                            <asp:FileUpload ID="fuModuleVideo" runat="server" CssClass="form-control-file" />
                            <asp:Label ID="lblCurrentVideo" runat="server" CssClass="text-muted d-block mt-1" Visible="false" />
                            <small class="text-muted">Optional MP4, WebM, OGG, or MOV (max 100 MB). Replaces the previous uploaded file.</small>
                        </div>
                        <div class="form-row">
                            <div class="form-group col-6">
                                <label>Module Order</label>
                                <asp:TextBox ID="txtOrder" runat="server" CssClass="form-control" TextMode="Number" />
                            </div>
                            <div class="form-group col-6">
                                <label>Estimated Hours</label>
                                <asp:TextBox ID="txtHours" runat="server" CssClass="form-control" TextMode="Number" />
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group col-6">
                                <label>Category</label>
                                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="form-group col-6">
                                <label>Instructor (Expert)</label>
                                <asp:DropDownList ID="ddlInstructor" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group">
                            <label>Thumbnail URL</label>
                            <asp:TextBox ID="txtThumbnailUrl" runat="server" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:CheckBox ID="chkIsMandatory" runat="server" />
                            <label for="chkIsMandatory">Mandatory Module</label>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" />
                            <label for="chkIsActive">Active</label>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnSaveModule" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveModule_Click" />
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
        .modules-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 20px;
        }
        .admin-module-card {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            border-top: 4px solid #667eea;
        }
        .module-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 10px;
        }
        .module-header h3 {
            margin: 0;
            font-size: 18px;
            color: #2d3748;
        }
        .module-description {
            color: #718096;
            font-size: 14px;
            line-height: 1.6;
            margin-bottom: 12px;
        }
        .module-meta {
            display: flex;
            gap: 15px;
            font-size: 13px;
            color: #718096;
            margin-bottom: 12px;
            flex-wrap: wrap;
        }
        .module-meta i {
            margin-right: 3px;
        }
        .module-actions {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
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
            .modules-grid {
                grid-template-columns: 1fr;
            }
            .filter-bar {
                flex-direction: column;
            }
            .form-row {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>