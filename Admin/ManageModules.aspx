<%@ Page Title="Manage Modules" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageModules.aspx.cs" Inherits="RespondX.Admin.ManageModules" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upModules" runat="server">
        <Triggers>
            <%-- File uploads need a full postback. --%>
            <asp:PostBackTrigger ControlID="btnSaveModule" />
        </Triggers>
        <ContentTemplate>
            <div class="container crud-region">
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

                <asp:Panel ID="pnlFilters" runat="server" CssClass="filter-bar" DefaultButton="btnSearch">
                    <div class="filter-group">
                        <label for="<%= ddlStatus.ClientID %>">Status:</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                            <asp:ListItem Value="All">All</asp:ListItem>
                            <asp:ListItem Value="Active">Active</asp:ListItem>
                            <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="filter-group">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search modules" />
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                    </div>
                    <div class="filter-group">
                        <asp:Button ID="btnAddModule" runat="server" Text="+ Add Module" CssClass="btn btn-success" OnClick="btnAddModule_Click" />
                    </div>
                </asp:Panel>

                <div class="crud-grid">
                    <asp:Repeater ID="rptModules" runat="server" OnItemCommand="rptModules_ItemCommand">
                        <ItemTemplate>
                            <div class="crud-card module-admin-card <%# (bool)Eval("IsActive") ? "" : "is-inactive" %>">
                                <img class="module-admin-thumb" src="<%# ResolveUrl(GetThumbnail(Eval("ThumbnailUrl"))) %>" alt="" loading="lazy"
                                     onerror="this.onerror=null;this.src='<%= ResolveUrl("~/Content/Images/default-module.svg") %>';" />
                                <div class="crud-card-header">
                                    <h3><%#: Eval("Title") %></h3>
                                    <span class="badge <%# (bool)Eval("IsActive") ? "badge-success" : "badge-secondary" %>"><%# (bool)Eval("IsActive") ? "Active" : "Inactive" %></span>
                                </div>
                                <p class="crud-card-body"><%#: Eval("Description") %></p>
                                <div class="crud-card-meta">
                                    <span><i class="fas fa-list"></i> <%# Eval("LessonCount") %> lessons</span>
                                    <span><i class="fas fa-clock"></i> <%# Eval("EstimatedHours") %> hours</span>
                                    <span><i class="fas fa-tags"></i> <%#: Eval("CategoryName") %></span>
                                </div>
                                <div class="crud-card-actions">
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="EditModule" CommandArgument='<%# Eval("ModuleID") %>' />
                                    <asp:Button ID="btnLessons" runat="server" Text="Lessons" CssClass="btn btn-primary btn-sm" CommandName="Lessons" CommandArgument='<%# Eval("ModuleID") %>' />
                                    <asp:Button ID="btnQuizzes" runat="server" Text="Quizzes" CssClass="btn btn-secondary btn-sm" CommandName="Quizzes" CommandArgument='<%# Eval("ModuleID") %>' />
                                    <asp:Button ID="btnScenarios" runat="server" Text="Scenarios" CssClass="btn btn-secondary btn-sm" CommandName="Scenarios" CommandArgument='<%# Eval("ModuleID") %>' />
                                    <asp:Button ID="btnToggle" runat="server" Text='<%# (bool)Eval("IsActive") ? "Deactivate" : "Activate" %>' CssClass='<%# (bool)Eval("IsActive") ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="ToggleModule" CommandArgument='<%# Eval("ModuleID") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="DeleteModule" CommandArgument='<%# Eval("ModuleID") %>' OnClientClick="return confirm('Delete this module? This only works for modules without lessons, quizzes or learner activity.');" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlNoModules" runat="server" CssClass="empty-state" Visible="false">
                        <img runat="server" src="~/Content/Images/empty-state.svg" alt="" />
                        No modules found.
                    </asp:Panel>
                </div>

                <div class="modal fade" id="modalModule" tabindex="-1" role="dialog" aria-hidden="true">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Module"></asp:Label></h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfModuleID" runat="server" />
                                <asp:Panel ID="pnlModalError" runat="server" CssClass="alert alert-danger" Visible="false">
                                    <asp:Label ID="lblModalError" runat="server"></asp:Label>
                                </asp:Panel>
                                <div class="form-group">
                                    <label for="<%= txtTitle.ClientID %>">Title <span class="required">*</span></label>
                                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="100" />
                                </div>
                                <div class="form-group">
                                    <label for="<%= txtDescription.ClientID %>">Module Description</label>
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="500"
                                        placeholder="Describe what learners will learn in this module" />
                                </div>
                                <div class="form-group">
                                    <label for="<%= txtYouTubeVideoUrl.ClientID %>">YouTube Video URL</label>
                                    <asp:TextBox ID="txtYouTubeVideoUrl" runat="server" CssClass="form-control" MaxLength="500"
                                        placeholder="https://www.youtube.com/watch?v=..." />
                                    <small class="form-text">Paste a YouTube link to show on the learner module page.</small>
                                </div>
                                <div class="form-group">
                                    <label for="<%= fuModuleVideo.ClientID %>">Upload Module Video</label>
                                    <asp:FileUpload ID="fuModuleVideo" runat="server" CssClass="form-control-file" accept=".mp4,.webm,.ogg,.mov,video/*" />
                                    <asp:Label ID="lblCurrentVideo" runat="server" CssClass="form-text" Visible="false" />
                                    <small class="form-text">Optional MP4, WebM, OGG, or MOV (max 100 MB). Replaces the previous uploaded file.</small>
                                </div>
                                <div class="form-row">
                                    <div class="form-group">
                                        <label for="<%= txtOrder.ClientID %>">Module Order</label>
                                        <asp:TextBox ID="txtOrder" runat="server" CssClass="form-control" TextMode="Number" min="1" />
                                    </div>
                                    <div class="form-group">
                                        <label for="<%= txtHours.ClientID %>">Estimated Hours</label>
                                        <asp:TextBox ID="txtHours" runat="server" CssClass="form-control" TextMode="Number" min="0" />
                                    </div>
                                </div>
                                <div class="form-row">
                                    <div class="form-group">
                                        <label for="<%= ddlCategory.ClientID %>">Category</label>
                                        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <label for="<%= ddlInstructor.ClientID %>">Instructor (Expert)</label>
                                        <asp:DropDownList ID="ddlInstructor" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="<%= txtThumbnailUrl.ClientID %>">Thumbnail Image URL</label>
                                    <asp:TextBox ID="txtThumbnailUrl" runat="server" CssClass="form-control" MaxLength="500" placeholder="https://... (leave blank for the default cover image)" />
                                </div>
                                <div class="form-row">
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkIsMandatory" runat="server" Text="Mandatory module" CssClass="form-check-inline" />
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" Text="Active" CssClass="form-check-inline" />
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnSaveModule" runat="server" Text="Save Module" CssClass="btn btn-primary" OnClick="btnSaveModule_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <style>
        .module-admin-card { padding-top: 0; overflow: hidden; }
        .module-admin-thumb {
            display: block;
            width: calc(100% + 40px);
            height: 150px;
            margin: 0 -20px 16px;
            object-fit: cover;
            background: #16213e;
        }
    </style>
</asp:Content>
