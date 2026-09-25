<%@ Page Title="Manage Quizzes - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageQuizzes.aspx.cs" Inherits="RespondX.Admin.ManageQuizzes" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upQuizzes" runat="server">
        <ContentTemplate>
            <div class="container crud-region">
                <div class="page-header">
                    <h1><i class="fas fa-question-circle"></i> Manage Quizzes</h1>
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
                        <asp:Button ID="btnAddQuiz" runat="server" Text="+ Add Quiz" CssClass="btn btn-success" OnClick="btnAddQuiz_Click" />
                    </div>
                </div>

                <div class="quizzes-grid">
                    <asp:Repeater ID="rptQuizzes" runat="server" OnItemCommand="rptQuizzes_ItemCommand">
                        <ItemTemplate>
                            <div class="quiz-card admin-quiz-card">
                                <div class="quiz-header">
                                    <h3><%# Eval("Title") %></h3>
                                    <span class="badge <%# Convert.ToBoolean(Eval("IsActive")) ? "badge-success" : "badge-secondary" %>">
                                        <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                                    </span>
                                </div>
                                <p class="quiz-description"><%# Eval("Description") %></p>
                                <div class="quiz-meta">
                                    <span><i class="fas fa-clock"></i> <%# Eval("TimeLimitMinutes") %> min</span>
                                    <span><i class="fas fa-check-circle"></i> Passing: <%# Eval("PassingScore") %>%</span>
                                    <span><i class="fas fa-redo"></i> Max Attempts: <%# Eval("MaxAttempts") %></span>
                                    <span><i class="fas fa-question"></i> Questions: <%# Eval("QuestionCount") %></span>
                                </div>
                                <div class="quiz-actions">
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" CommandName="Edit" CommandArgument='<%# Eval("QuizID") %>' />
                                    <asp:Button ID="btnQuestions" runat="server" Text="Questions" CssClass="btn btn-primary btn-sm" CommandName="Questions" CommandArgument='<%# Eval("QuizID") %>' />
                                    <asp:Button ID="btnToggle" runat="server" Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-warning btn-sm" : "btn btn-success btn-sm" %>' CommandName="Toggle" CommandArgument='<%# Eval("QuizID") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Deactivate" CssClass="btn btn-danger btn-sm" CommandName="Delete" CommandArgument='<%# Eval("QuizID") %>' OnClientClick="return confirm('Deactivate this quiz? Existing learner results will be preserved.');" />
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Label ID="lblNoQuizzes" runat="server" Text="No quizzes found" Visible='<%# rptQuizzes.Items.Count == 0 %>' CssClass="text-muted text-center d-block" />
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="modal fade" id="modalQuiz" tabindex="-1">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Add Quiz"></asp:Label></h5>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfQuizID" runat="server" />
                                <asp:HiddenField ID="hfModuleID" runat="server" />
                                <div class="form-group">
                                    <label>Title *</label>
                                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                                </div>
                                <div class="form-group">
                                    <label>Description</label>
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                                </div>
                                <div class="form-row">
                                    <div class="form-group col-6">
                                        <label>Time Limit (Minutes)</label>
                                        <asp:TextBox ID="txtTimeLimit" runat="server" CssClass="form-control" TextMode="Number" />
                                    </div>
                                    <div class="form-group col-6">
                                        <label>Passing Score (%)</label>
                                        <asp:TextBox ID="txtPassingScore" runat="server" CssClass="form-control" TextMode="Number" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label>Max Attempts</label>
                                    <asp:TextBox ID="txtMaxAttempts" runat="server" CssClass="form-control" TextMode="Number" />
                                </div>
                                <div class="form-group">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" Text="Active" CssClass="form-check-inline" />
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnSaveQuiz" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveQuiz_Click" />
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
        .quizzes-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 20px;
        }
        .admin-quiz-card {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            border-top: 4px solid #667eea;
        }
        .quiz-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 10px;
        }
        .quiz-header h3 {
            margin: 0;
            font-size: 18px;
            color: #2d3748;
        }
        .quiz-description {
            color: #718096;
            font-size: 14px;
            line-height: 1.6;
            margin-bottom: 12px;
        }
        .quiz-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            font-size: 13px;
            color: #718096;
            margin-bottom: 12px;
        }
        .quiz-actions {
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
        .form-row {
            display: flex;
            gap: 15px;
        }
        .form-row .form-group {
            flex: 1;
        }
        @media (max-width: 768px) {
            .quizzes-grid {
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
