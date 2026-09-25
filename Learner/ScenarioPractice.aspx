<%@ Page Title="Scenario Practice" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ScenarioPractice.aspx.cs" Inherits="RespondX.Learner.ScenarioPractice" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Panel ID="pnlScenario" runat="server" Visible="false">
            <asp:HiddenField ID="hfScenarioId" runat="server" />
            <div class="scenario-container">
                <div class="scenario-header">
                    <div class="scenario-title">
                        <h1><asp:Label ID="lblTitle" runat="server"></asp:Label></h1>
                        <span class="badge badge-info"><asp:Label ID="lblDifficulty" runat="server"></asp:Label></span>
                    </div>
                    <p class="text-muted"><asp:Label ID="lblDescription" runat="server"></asp:Label></p>
                </div>

                <div class="scenario-body">
                    <div class="scenario-text">
                        <asp:Literal ID="litScenarioText" runat="server"></asp:Literal>
                    </div>

                    <asp:UpdatePanel ID="upAnswer" runat="server">
                        <ContentTemplate>
                            <div class="scenario-options">
                                <h4>What would you do?</h4>
                                <asp:Repeater ID="rptOptions" runat="server">
                                    <ItemTemplate>
                                        <label class="scenario-option <%# Eval("CssClass") %>">
                                            <span class="option-row">
                                                <input type="radio" name="scenarioOption" value="<%# Eval("OptionID") %>"
                                                    <%# (bool)Eval("IsSelected") ? "checked=\"checked\"" : "" %> <%# (bool)Eval("IsLocked") ? "disabled=\"disabled\"" : "" %> />
                                                <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrEmpty((string)Eval("IconUrl")) %>'>
                                                    <img src="<%# ResolveUrl((string)Eval("IconUrl") ?? "~/") %>" alt="" class="img-icon" />
                                                </asp:PlaceHolder>
                                                <span><%#: Eval("OptionText") %></span>
                                            </span>
                                            <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrEmpty((string)Eval("Feedback")) %>'>
                                                <span class="option-feedback"><span class="feedback-text"><%#: Eval("Feedback") %></span></span>
                                            </asp:PlaceHolder>
                                        </label>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <asp:Panel ID="pnlResult" runat="server" Visible="false">
                                <img id="imgResult" runat="server" alt="" />
                                <asp:Label ID="lblResult" runat="server" />
                            </asp:Panel>

                            <div class="scenario-actions">
                                <asp:Button ID="btnSubmitAnswer" runat="server" Text="Submit Answer" CssClass="btn btn-primary" OnClick="btnSubmitAnswer_Click" />
                                <asp:Button ID="btnTryAgain" runat="server" Text="Try Again" CssClass="btn btn-secondary" OnClick="btnTryAgain_Click" Visible="false" />
                                <asp:HyperLink ID="hlNextScenario" runat="server" CssClass="btn btn-success" Visible="false">
                                    <asp:Literal ID="litNextText" runat="server" Text="Next Scenario" /><img runat="server" src="~/Content/Images/icons/arrow-right.svg" alt="" class="img-icon" />
                                </asp:HyperLink>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="status-page">
                <img runat="server" src="~/Content/Images/not-found.svg" alt="" class="status-illustration" />
                <h1>Scenario not found</h1>
                <p>This scenario doesn't exist or isn't available for practice yet.</p>
                <div class="status-actions">
                    <a runat="server" href="~/Learner/Scenarios.aspx" class="btn btn-primary">Back to Scenarios</a>
                </div>
            </div>
        </asp:Panel>
    </div>

    <style>
        .scenario-container {
            background: white;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .scenario-title {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 10px;
        }
        .scenario-title h1 {
            margin: 0;
        }
        .scenario-progress {
            margin: 15px 0;
        }
        .scenario-progress span {
            display: block;
            margin-bottom: 5px;
            font-weight: 600;
        }
        .scenario-text {
            padding: 20px;
            background: #f7fafc;
            border-radius: 8px;
            margin: 20px 0;
            line-height: 1.8;
        }
        .scenario-options {
            margin: 25px 0;
        }
        .scenario-options h4 {
            margin-bottom: 15px;
        }
        .scenario-option {
            padding: 15px 20px;
            border: 2px solid #e2e8f0;
            border-radius: 8px;
            margin-bottom: 10px;
            cursor: pointer;
            transition: all 0.3s;
            display: flex;
            flex-direction: column;
            gap: 5px;
        }
        .scenario-option:hover {
            border-color: var(--accent);
            background: #f7fafc;
        }
        .scenario-option.selected {
            border-color: var(--accent);
            background: var(--accent-tint);
        }
        .scenario-option.correct {
            border-color: var(--success);
            background: var(--success-tint);
        }
        .scenario-option.incorrect {
            border-color: var(--danger);
            background: var(--danger-tint);
        }
        .scenario-option input[type="radio"] {
            width: 18px;
            height: 18px;
            margin: 0;
            accent-color: var(--accent);
        }
        .scenario-option label {
            cursor: pointer;
            margin: 0;
            font-weight: 500;
        }
        .option-feedback {
            margin-top: 8px;
            padding: 10px;
            border-radius: 5px;
            font-size: 14px;
        }
        .option-feedback .feedback-text {
            display: block;
        }
        .correct .option-feedback {
            background: #c6f6d5;
            color: #22543d;
        }
        .incorrect .option-feedback {
            background: #fed7d7;
            color: #9b2c2c;
        }
        .scenario-actions {
            display: flex;
            gap: 10px;
            margin-top: 20px;
        }
        .scenario-actions .btn:last-child {
            margin-left: auto;
        }
        .scenario-option.is-locked {
            cursor: default;
        }
        .scenario-option .option-row {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .scenario-option .option-row .img-icon {
            width: 20px;
            height: 20px;
        }
        .scenario-result {
            display: flex;
            align-items: center;
            gap: 12px;
            margin-top: 10px;
            padding: 14px 16px;
            border-radius: 8px;
            font-weight: 600;
        }
        .scenario-result img {
            width: 28px;
            height: 28px;
        }
        .scenario-result.correct {
            background: var(--success-tint);
            color: var(--success);
        }
        .scenario-result.incorrect {
            background: var(--danger-tint);
            color: var(--danger);
        }
    </style>

    <script>
        // Highlight the chosen option before submitting (options are native radio buttons inside labels).
        document.addEventListener('change', function (event) {
            if (event.target.name !== 'scenarioOption') return;
            document.querySelectorAll('.scenario-option').forEach(function (option) {
                option.classList.toggle('selected', option.contains(event.target));
            });
        });
    </script>
</asp:Content>
