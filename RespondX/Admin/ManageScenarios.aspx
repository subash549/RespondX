<%@ Page Title="Manage Scenarios - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageScenarios.aspx.cs" Inherits="RespondX.Admin.ManageScenarios" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Admin.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-users"></i> Manage Scenarios</h1>
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
                <asp:Button ID="btnBack" runat="server" Text="← Back to Modules" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
            </div>
            <div class="filter-group">
                <label>Difficulty:</label>
                <asp:DropDownList ID="ddlDifficulty" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDifficulty_SelectedIndexChanged">
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

        <div class="scenarios-grid">
            <asp:Repeater ID="rptScenarios" runat="server" OnItemCommand="rptScenarios_ItemCommand">
                <ItemTemplate>
                    <div class="scenario-card">
                        <div class="scenario-header">
                            <h3><%# Eval("Title") %></h3>
                            <span class="badge badge-primary">Difficulty: <%# Eval("DifficultyDisplay") %></span>
                        </div>
                        <p class="scenario-description"><%# Eval("Description") %></p>
                        <div class="scenario-meta">
                            <span><i class="fas fa-list"></i> Order: <%# Eval("ScenarioOrder") %></span>
                            <span><i class="fas fa-check-circle"></i> Options: <%# Eval("OptionsCount") %></span>
                            <span class="badge <%# Convert.ToBoolean(Eval("IsActive")) ? "badge-success" : "badge-secondary" %>">
                                <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                            </span>
                        </div>
                        <div class="scenario-actions">
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("ScenarioID") %>' />
                            <asp:Button ID="btnOptions" runat="server" Text="Options" CssClass="btn btn-primary btn-sm" CommandName="Options" CommandArgument='<%# Eval("ScenarioID") %>' />
                            <asp:Button ID="btnToggle" runat="server" Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="Toggle" CommandArgument='<%# Eval("ScenarioID") %>' />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="Delete" CommandArgument='<%# Eval("ScenarioID") %>' OnClientClick="return confirm('Are you sure you want to delete this scenario?');" />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoScenarios" runat="server" Text="No scenarios found" Visible='<%# rptScenarios.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="modal fade" id="modalScenario" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Scenario"></asp:Label></h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfScenarioID" runat="server" />
                        <asp:HiddenField ID="hfModuleID" runat="server" />
                        <div class="form-group">
                            <label>Title *</label>
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <label>Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                        </div>
                        <div class="form-group">
                            <label>Scenario Text *</label>
                            <asp:TextBox ID="txtScenarioText" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
                        </div>
                        <div class="form-row">
                            <div class="form-group col-6">
                                <label>Scenario Order</label>
                                <asp:TextBox ID="txtOrder" runat="server" CssClass="form-control" TextMode="Number" />
                            </div>
                            <div class="form-group col-6">
                                <label>Difficulty Level</label>
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
                            <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" />
                            <label for="chkIsActive">Active</label>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnSaveScenario" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveScenario_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="modalOptions" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Manage Options</h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfOptionsScenarioID" runat="server" />
                        <asp:Repeater ID="rptOptions" runat="server" OnItemCommand="rptOptions_ItemCommand">
                            <HeaderTemplate>
                                <div class="options-list">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="option-item">
                                    <div class="option-text">
                                        <strong><%# Eval("OptionOrder") %>.</strong> <%# Eval("OptionText") %>
                                        <span class="badge <%# Convert.ToBoolean(Eval("IsCorrect")) ? "badge-success" : "badge-secondary" %>">
                                            <%# Convert.ToBoolean(Eval("IsCorrect")) ? "Correct" : "Incorrect" %>
                                        </span>
                                    </div>
                                    <div class="option-actions">
                                        <asp:Button ID="btnEditOption" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="EditOption" CommandArgument='<%# Eval("OptionID") %>' />
                                        <asp:Button ID="btnDeleteOption" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" CommandName="DeleteOption" CommandArgument='<%# Eval("OptionID") %>' OnClientClick="return confirm('Are you sure?');" />
                                    </div>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                </div>
                                <asp:Label ID="lblNoOptions" runat="server" Text="No options added yet" Visible='<%# rptOptions.Items.Count == 0 %>' CssClass="text-muted" />
                            </FooterTemplate>
                        </asp:Repeater>
                        <hr />
                        <div class="form-group">
                            <label>Add New Option</label>
                            <asp:TextBox ID="txtOptionText" runat="server" CssClass="form-control" placeholder="Option text" />
                        </div>
                        <div class="form-group">
                            <asp:CheckBox ID="chkIsCorrectOption" runat="server" />
                            <label for="chkIsCorrectOption">Is Correct Answer</label>
                        </div>
                        <asp:Button ID="btnAddOption" runat="server" Text="Add Option" CssClass="btn btn-primary" OnClick="btnAddOption_Click" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <style>
        .page-header {
            margin-bottom: 30px;
        }
        .scenarios-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 20px;
        }
        .scenario-card {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            border-top: 4px solid #9f7aea;
        }
        .scenario-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 10px;
        }
        .scenario-header h3 {
            margin: 0;
            font-size: 18px;
            color: #2d3748;
        }
        .scenario-description {
            color: #718096;
            font-size: 14px;
            line-height: 1.6;
            margin-bottom: 12px;
        }
        .scenario-meta {
            display: flex;
            gap: 15px;
            font-size: 13px;
            color: #718096;
            margin-bottom: 12px;
            flex-wrap: wrap;
        }
        .scenario-actions {
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
        .option-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 0;
            border-bottom: 1px solid #f7fafc;
        }
        .option-item:last-child {
            border-bottom: none;
        }
        .option-text {
            flex: 1;
        }
        .option-actions {
            display: flex;
            gap: 5px;
        }
        .options-list {
            max-height: 300px;
            overflow-y: auto;
        }
        .form-row {
            display: flex;
            gap: 15px;
        }
        .form-row .form-group {
            flex: 1;
        }
        @media (max-width: 768px) {
            .scenarios-grid {
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