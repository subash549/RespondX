<%@ Page Title="Manage Questions - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageQuestions.aspx.cs" Inherits="RespondX.Admin.ManageQuestions" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upQuestions" runat="server">
        <ContentTemplate>
            <div class="container crud-region">
                <div class="page-header">
                    <h1><i class="fas fa-list"></i> Manage Questions</h1>
                    <p><asp:Label ID="lblQuizInfo" runat="server"></asp:Label></p>
                    <p class="text-muted">Every module has a quiz here. A default quiz is created for modules that do not have one yet. Use the Quizzes button on the Modules page to adjust its settings.</p>
                    <p class="text-muted">Built-in questions remain available to learners. Add questions or edit saved database questions for the selected quiz.</p>
                </div>

                <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
                    <asp:Label ID="lblSuccess" runat="server"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </asp:Panel>

                <div class="filter-bar">
                    <div class="filter-group">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-secondary" OnClick="btnBack_Click"><img runat="server" src="~/Content/Images/icons/arrow-left.svg" alt="" class="img-icon" />Back to Admin</asp:LinkButton>
                        <label>Quiz</label>
                        <asp:DropDownList ID="ddlQuiz" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlQuiz_SelectedIndexChanged" />
                    </div>
                    <div class="filter-group">
                        <asp:Button ID="btnAddQuestion" runat="server" Text="+ Add Question" CssClass="btn btn-success" OnClick="btnAddQuestion_Click" UseSubmitBehavior="false" />
                    </div>
                </div>

                <div class="questions-list">
                    <asp:Repeater ID="rptQuestions" runat="server" OnItemCommand="rptQuestions_ItemCommand">
                        <ItemTemplate>
                            <div class="question-item">
                                <div class="question-header">
                                    <div class="question-info">
                                        <span class="question-number">Q<%# Container.ItemIndex + 1 %>.</span>
                                        <span class="question-text"><%# Server.HtmlEncode(Convert.ToString(Eval("QuestionText"))) %></span>
                                    </div>
                                    <div class="question-badges">
                                        <span class="badge badge-info"><%# Eval("QuestionTypeDisplay") %></span>
                                        <span class="badge badge-secondary"><%# Eval("Points") %> pts</span>
                                        <span class="badge badge-success">Saved</span>
                                    </div>
                                </div>
                                <div class="question-options">
                                    <asp:Repeater ID="rptOptions" runat="server" DataSource='<%# Eval("Options") %>'>
                                        <ItemTemplate>
                                            <div class="option-item-small">
                                                <span class="option-label"><%# Eval("OptionLabel") %>.</span>
                                                <span class="option-text"><%# Server.HtmlEncode(Convert.ToString(Eval("OptionText"))) %></span>
                                                <span class="badge <%# Convert.ToBoolean(Eval("IsCorrect")) ? "badge-success" : "badge-secondary" %>">
                                                    <%# Convert.ToBoolean(Eval("IsCorrect")) ? "Correct answer" : "" %>
                                                </span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <div class="question-actions">
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit question" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("QuestionID") %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Label ID="lblNoQuestions" runat="server" Text="No admin-added questions for this quiz yet. Built-in questions remain available to learners." Visible='<%# rptQuestions.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="modal fade" id="modalQuestion" tabindex="-1">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Question"></asp:Label></h5>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfQuestionID" runat="server" />
                                <asp:HiddenField ID="hfQuizID" runat="server" />
                                <div class="form-group">
                                    <label>Question Text *</label>
                                    <asp:TextBox ID="txtQuestionText" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                                </div>
                                <div class="form-row">
                                    <div class="form-group col-6">
                                        <label>Question Type</label>
                                        <div class="form-control-plaintext">Multiple choice</div>
                                    </div>
                                    <div class="form-group col-6">
                                        <label>Points</label>
                                        <asp:TextBox ID="txtPoints" runat="server" CssClass="form-control" TextMode="Number" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label>Four answer choices (select one correct answer)</label>
                                    <div id="optionsContainer">
                                        <div class="option-input">
                                            <asp:TextBox ID="txtOption1" runat="server" CssClass="form-control" placeholder="Option A" />
                                            <asp:RadioButton ID="rdoCorrect1" runat="server" GroupName="correctAnswer" Text="Correct answer" CssClass="ml-1" />
                                        </div>
                                        <div class="option-input mt-1">
                                            <asp:TextBox ID="txtOption2" runat="server" CssClass="form-control" placeholder="Option B" />
                                            <asp:RadioButton ID="rdoCorrect2" runat="server" GroupName="correctAnswer" Text="Correct answer" CssClass="ml-1" />
                                        </div>
                                        <div class="option-input mt-1">
                                            <asp:TextBox ID="txtOption3" runat="server" CssClass="form-control" placeholder="Option C" />
                                            <asp:RadioButton ID="rdoCorrect3" runat="server" GroupName="correctAnswer" Text="Correct answer" CssClass="ml-1" />
                                        </div>
                                        <div class="option-input mt-1">
                                            <asp:TextBox ID="txtOption4" runat="server" CssClass="form-control" placeholder="Option D" />
                                            <asp:RadioButton ID="rdoCorrect4" runat="server" GroupName="correctAnswer" Text="Correct answer" CssClass="ml-1" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnSaveQuestion" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveQuestion_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <style>
        .page-header {
            margin-bottom: 30px;
        }
        .questions-list {
            display: grid;
            gap: 20px;
        }
        .question-item {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            border-left: 4px solid #667eea;
        }
        .question-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 10px;
            flex-wrap: wrap;
            gap: 10px;
        }
        .question-info {
            display: flex;
            gap: 10px;
            align-items: flex-start;
        }
        .question-number {
            font-weight: 700;
            color: #667eea;
        }
        .question-text {
            font-weight: 500;
            color: #2d3748;
        }
        .question-badges {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }
        .question-options {
            margin: 10px 0 10px 30px;
            display: grid;
            gap: 5px;
        }
        .option-item-small {
            display: flex;
            gap: 10px;
            align-items: center;
            padding: 3px 0;
        }
        .option-label {
            font-weight: 600;
            color: #4a5568;
            min-width: 20px;
        }
        .option-text {
            color: #4a5568;
        }
        .question-actions {
            display: flex;
            gap: 8px;
            margin-top: 10px;
            flex-wrap: wrap;
        }
        .option-input {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .option-input .form-control {
            flex: 1;
        }
        .option-input input[type="radio"] {
            width: 18px;
            height: 18px;
            cursor: pointer;
        }
        .option-input label {
            margin: 0;
            cursor: pointer;
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
        .form-row {
            display: flex;
            gap: 15px;
        }
        .form-row .form-group {
            flex: 1;
        }
        .mt-1 {
            margin-top: 8px;
        }
        .ml-1 {
            margin-left: 8px;
        }
        @media (max-width: 768px) {
            .question-header {
                flex-direction: column;
            }
            .question-options {
                margin-left: 0;
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
