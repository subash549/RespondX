<%@ Page Title="Scenario Practice - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ScenarioPractice.aspx.cs" Inherits="RespondX.Learner.ScenarioPractice" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Panel ID="pnlScenario" runat="server" Visible="false">
            <div class="scenario-container">
                <div class="scenario-header">
                    <div class="scenario-title">
                        <h1><asp:Label ID="lblTitle" runat="server"></asp:Label></h1>
                        <span class="badge badge-info"><asp:Label ID="lblDifficulty" runat="server"></asp:Label></span>
                    </div>
                    <p><asp:Label ID="lblDescription" runat="server"></asp:Label></p>
                    <div class="scenario-progress">
                        <span>Progress: <asp:Label ID="lblProgress" runat="server" Text="0%"></asp:Label></span>
                        <div class="progress">
                            <div class="progress-bar" style="width: <asp:Label ID="lblProgressBar" runat="server" Text="0"></asp:Label>%"></div>
                        </div>
                    </div>
                </div>

                <div class="scenario-body">
                    <div class="scenario-text">
                        <asp:Literal ID="litScenarioText" runat="server"></asp:Literal>
                    </div>

                    <div class="scenario-options">
                        <h4>What would you do?</h4>
                        <asp:Repeater ID="rptOptions" runat="server" OnItemCommand="rptOptions_ItemCommand">
                            <ItemTemplate>
                                <div class="scenario-option" onclick="selectOption(this)">
                                    <input type="radio" name="scenarioOption" value='<%# Eval("OptionID") %>' id='opt_<%# Eval("OptionID") %>' />
                                    <label for='opt_<%# Eval("OptionID") %>'><%# Eval("OptionText") %></label>
                                    <div class="option-feedback" style="display:none;">
                                        <span class="feedback-text"><%# Eval("Feedback") %></span>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="scenario-actions">
                        <asp:Button ID="btnSubmitAnswer" runat="server" Text="Submit Answer" CssClass="btn btn-primary" OnClick="btnSubmitAnswer_Click" />
                        <asp:Button ID="btnNextScenario" runat="server" Text="Next Scenario →" CssClass="btn btn-success" OnClick="btnNextScenario_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="alert alert-warning">
                <h4>Scenario Not Found</h4>
                <p>The scenario you're looking for doesn't exist.</p>
                <a href="Scenarios.aspx" class="btn btn-primary">Back to Scenarios</a>
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
            border-color: #667eea;
            background: #f7fafc;
        }
        .scenario-option.selected {
            border-color: #667eea;
            background: #ebf4ff;
        }
        .scenario-option.correct {
            border-color: #48bb78;
            background: #f0fff4;
        }
        .scenario-option.incorrect {
            border-color: #fc8181;
            background: #fff5f5;
        }
        .scenario-option input[type="radio"] {
            display: none;
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
        .scenario-actions .btn-next {
            margin-left: auto;
        }
    </style>

    <script>
        function selectOption(element) {
            // Remove selection from other options
            var parent = element.closest('.scenario-options');
            parent.querySelectorAll('.scenario-option').forEach(function(opt) {
                opt.classList.remove('selected');
            });
            element.classList.add('selected');
            element.querySelector('input[type="radio"]').checked = true;
        }

        // Show feedback for selected option
        function showFeedback(element, isCorrect, feedback) {
            element.classList.add(isCorrect ? 'correct' : 'incorrect');
            var feedbackDiv = element.querySelector('.option-feedback');
            feedbackDiv.style.display = 'block';
            feedbackDiv.querySelector('.feedback-text').textContent = feedback;
            
            // Disable all options
            var parent = element.closest('.scenario-options');
            parent.querySelectorAll('.scenario-option input[type="radio"]').forEach(function(input) {
                input.disabled = true;
            });
            
            // Enable next button
            document.querySelector('.btn-next').style.display = 'inline-block';
        }
    </script>
</asp:Content>