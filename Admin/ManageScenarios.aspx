<%@ Page Title="Manage Scenarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageScenarios.aspx.cs" Inherits="RespondX.Admin.ManageScenarios" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upScenarios" runat="server">
        <ContentTemplate>
            <div class="container crud-region">
                <div class="page-header">
                    <h1><i class="fas fa-users"></i> Manage Scenarios</h1>
                    <p>Build real-world practice scenarios and their answer options</p>
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
                        <label for="<%= ddlModuleFilter.ClientID %>">Module:</label>
                        <asp:DropDownList ID="ddlModuleFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" />
                    </div>
                    <div class="filter-group">
                        <label for="<%= ddlDifficulty.ClientID %>">Difficulty:</label>
                        <asp:DropDownList ID="ddlDifficulty" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                            <asp:ListItem Value="All">All</asp:ListItem>
                            <asp:ListItem Value="1">Beginner</asp:ListItem>
                            <asp:ListItem Value="2">Easy</asp:ListItem>
                            <asp:ListItem Value="3">Intermediate</asp:ListItem>
                            <asp:ListItem Value="4">Advanced</asp:ListItem>
                            <asp:ListItem Value="5">Expert</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="filter-group">
                        <asp:Button ID="btnAddScenario" runat="server" Text="+ Add Scenario" CssClass="btn btn-success" OnClick="btnAddScenario_Click" />
                    </div>
                </div>

                <div class="crud-grid">
                    <asp:Repeater ID="rptScenarios" runat="server" OnItemCommand="rptScenarios_ItemCommand">
                        <ItemTemplate>
                            <div class="crud-card <%# (bool)Eval("IsActive") ? "" : "is-inactive" %>">
                                <div class="crud-card-header">
                                    <h3><%#: Eval("Title") %></h3>
                                    <span class="badge <%# (bool)Eval("IsActive") ? "badge-success" : "badge-secondary" %>"><%# (bool)Eval("IsActive") ? "Active" : "Inactive" %></span>
                                </div>
                                <p class="crud-card-body"><%#: Eval("Description") %></p>
                                <div class="crud-card-meta">
                                    <span><i class="fas fa-book"></i> <%#: Eval("Module") %></span>
                                    <span><i class="fas fa-signal"></i> <%#: Eval("DifficultyDisplay") %></span>
                                    <span><i class="fas fa-list-ul"></i> <%# Eval("OptionsCount") %> option(s)</span>
                                </div>
                                <div class="crud-card-actions">
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="EditScenario" CommandArgument='<%# Eval("ScenarioID") %>' />
                                    <asp:Button ID="btnOptions" runat="server" Text="Options" CssClass="btn btn-primary btn-sm" CommandName="Options" CommandArgument='<%# Eval("ScenarioID") %>' />
                                    <asp:Button ID="btnToggle" runat="server" Text='<%# (bool)Eval("IsActive") ? "Deactivate" : "Activate" %>' CssClass='<%# (bool)Eval("IsActive") ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="ToggleScenario" CommandArgument='<%# Eval("ScenarioID") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="DeleteScenario" CommandArgument='<%# Eval("ScenarioID") %>' OnClientClick="return confirm('Delete this scenario and all of its options?');" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlNoScenarios" runat="server" CssClass="empty-state" Visible="false">
                        <img runat="server" src="~/Content/Images/empty-state.svg" alt="" />
                        No scenarios yet. Use "Add Scenario" to create one.
                    </asp:Panel>
                </div>

                <%-- Add / edit scenario --%>
                <div class="modal fade" id="modalScenario" tabindex="-1" role="dialog" aria-hidden="true">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Scenario"></asp:Label></h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfScenarioID" runat="server" />
                                <asp:Panel ID="pnlModalError" runat="server" CssClass="alert alert-danger" Visible="false">
                                    <asp:Label ID="lblModalError" runat="server"></asp:Label>
                                </asp:Panel>
                                <div class="form-group">
                                    <label for="<%= ddlScenarioModule.ClientID %>">Module <span class="required">*</span></label>
                                    <asp:DropDownList ID="ddlScenarioModule" runat="server" CssClass="form-control" />
                                </div>
                                <div class="form-group">
                                    <label for="<%= txtTitle.ClientID %>">Title <span class="required">*</span></label>
                                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="100" />
                                </div>
                                <div class="form-group">
                                    <label for="<%= txtDescription.ClientID %>">Short Description</label>
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500" />
                                </div>
                                <div class="form-group">
                                    <label for="<%= txtScenarioText.ClientID %>">Scenario Text <span class="required">*</span></label>
                                    <asp:TextBox ID="txtScenarioText" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"
                                        placeholder="Describe the situation the learner faces" />
                                </div>
                                <div class="form-row">
                                    <div class="form-group">
                                        <label for="<%= txtOrder.ClientID %>">Scenario Order</label>
                                        <asp:TextBox ID="txtOrder" runat="server" CssClass="form-control" TextMode="Number" min="1" placeholder="Next in order" />
                                    </div>
                                    <div class="form-group">
                                        <label for="<%= ddlDifficultyLevel.ClientID %>">Difficulty Level</label>
                                        <asp:DropDownList ID="ddlDifficultyLevel" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="1">Beginner</asp:ListItem>
                                            <asp:ListItem Value="2">Easy</asp:ListItem>
                                            <asp:ListItem Value="3">Intermediate</asp:ListItem>
                                            <asp:ListItem Value="4">Advanced</asp:ListItem>
                                            <asp:ListItem Value="5">Expert</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" Text="Active" CssClass="form-check-inline" />
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnSaveScenario" runat="server" Text="Save Scenario" CssClass="btn btn-primary" OnClick="btnSaveScenario_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <%-- Answer options --%>
                <div class="modal fade" id="modalOptions" tabindex="-1" role="dialog" aria-hidden="true">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Options: <asp:Label ID="lblOptionsScenario" runat="server" /></h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfOptionsScenarioID" runat="server" />
                                <asp:HiddenField ID="hfOptionID" runat="server" />
                                <asp:Panel ID="pnlOptionError" runat="server" CssClass="alert alert-danger" Visible="false">
                                    <asp:Label ID="lblOptionError" runat="server"></asp:Label>
                                </asp:Panel>

                                <asp:Repeater ID="rptOptions" runat="server" OnItemCommand="rptOptions_ItemCommand">
                                    <HeaderTemplate><ul class="option-list"></HeaderTemplate>
                                    <ItemTemplate>
                                        <li class="<%# (bool)Eval("IsCorrect") ? "is-correct" : "" %>">
                                            <img src="<%# ResolveUrl((bool)Eval("IsCorrect") ? "~/Content/Images/icons/check.svg" : "~/Content/Images/icons/cross.svg") %>"
                                                 alt="<%# (bool)Eval("IsCorrect") ? "Correct" : "Incorrect" %>" class="img-icon" />
                                            <span class="option-text">
                                                <strong><%# Eval("OptionOrder") %>.</strong> <%#: Eval("OptionText") %>
                                                <span class="option-feedback"><%#: Eval("Feedback") %></span>
                                            </span>
                                            <asp:Button ID="btnEditOption" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="EditOption" CommandArgument='<%# Eval("OptionID") %>' />
                                            <asp:Button ID="btnDeleteOption" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="DeleteOption" CommandArgument='<%# Eval("OptionID") %>' OnClientClick="return confirm('Delete this option?');" />
                                        </li>
                                    </ItemTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                </asp:Repeater>
                                <asp:Label ID="lblNoOptions" runat="server" CssClass="text-muted d-block" Text="No options yet. Add at least two, and mark one as correct." Visible="false" />

                                <hr />
                                <h6><asp:Label ID="lblOptionFormTitle" runat="server" Text="Add an option" /></h6>
                                <asp:Panel ID="pnlOptionForm" runat="server" DefaultButton="btnSaveOption">
                                    <div class="form-group">
                                        <label for="<%= txtOptionText.ClientID %>">Option text <span class="required">*</span></label>
                                        <asp:TextBox ID="txtOptionText" runat="server" CssClass="form-control" placeholder="What the learner could do" />
                                    </div>
                                    <div class="form-group">
                                        <label for="<%= txtOptionFeedback.ClientID %>">Feedback shown after choosing it</label>
                                        <asp:TextBox ID="txtOptionFeedback" runat="server" CssClass="form-control" MaxLength="500" placeholder="Why this is right or wrong" />
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkIsCorrectOption" runat="server" Text="This is the correct answer" CssClass="form-check-inline" />
                                    </div>
                                    <asp:Button ID="btnSaveOption" runat="server" Text="Add Option" CssClass="btn btn-primary" OnClick="btnSaveOption_Click" />
                                    <asp:Button ID="btnCancelOptionEdit" runat="server" Text="Cancel Edit" CssClass="btn btn-secondary" OnClick="btnCancelOptionEdit_Click" Visible="false" />
                                </asp:Panel>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Done</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
