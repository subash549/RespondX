<%@ Page Title="Quizzes - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Quizzes.aspx.cs" Inherits="RespondX.Learner.Quizzes" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Learner.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h1><i class="fas fa-question-circle"></i> Quizzes</h1>
            <p>Test your knowledge and earn certifications</p>
        </div>

        <div class="filter-bar">
            <div class="filter-group">
                <label>Status:</label>
                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                    <asp:ListItem Value="All">All Quizzes</asp:ListItem>
                    <asp:ListItem Value="Available">Available</asp:ListItem>
                    <asp:ListItem Value="InProgress">In Progress</asp:ListItem>
                    <asp:ListItem Value="Completed">Completed</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search quizzes..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
        </div>

        <div class="quiz-grid">
            <asp:Repeater ID="rptQuizzes" runat="server">
                <ItemTemplate>
                    <div class="quiz-card">
                        <div class="quiz-header">
                            <h3><%# Eval("Title") %></h3>
                            <span class="badge badge-primary"><%# Eval("Module") %></span>
                        </div>
                        <p class="quiz-description"><%# Eval("Description") %></p>
                        <div class="quiz-meta">
                            <span><i class="fas fa-clock"></i> <%# Eval("TimeLimit") %> min</span>
                            <span><i class="fas fa-question"></i> <%# Eval("QuestionCount") %> questions</span>
                            <span><i class="fas fa-check-circle"></i> Passing: <%# Eval("PassingScore") %>%</span>
                            <span><i class="fas fa-redo"></i> Attempts: <%# Eval("Attempts") %>/<%# Eval("MaxAttempts") %></span>
                        </div>
                        <div class="quiz-status">
                            <span class="badge <%# Eval("StatusBadge") %>"><%# Eval("Status") %></span>
                            <%# Eval("Score") != null ? $"<span class='badge badge-info'>Score: {Eval("Score")}%</span>" : "" %>
                        </div>
                        <div class="quiz-actions">
                            <asp:Button ID="btnTakeQuiz" runat="server" Text='<%# Eval("ButtonText") %>' 
                                CssClass='<%# Eval("ButtonClass") %>' 
                                CommandArgument='<%# Eval("QuizID") %>' 
                                OnClick="btnTakeQuiz_Click" 
                                Enabled='<%# Eval("IsAvailable") %>' />
                            <asp:Button ID="btnReview" runat="server" Text="Review" 
                                CssClass="btn btn-info btn-sm" 
                                CommandArgument='<%# Eval("QuizID") %>' 
                                OnClick="btnReview_Click" 
                                Visible='<%# Eval("IsCompleted") %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblNoQuizzes" runat="server" Text="No quizzes found" Visible='<%# rptQuizzes.Items.Count == 0 %>' CssClass="text-muted text-center" />
                </FooterTemplate>
            </asp:Repeater>
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
        .quiz-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 25px;
            margin-top: 20px;
        }
        .quiz-card {
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
            transition: all 0.3s;
            border-top: 4px solid #667eea;
        }
        .quiz-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.12);
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
            margin-bottom: 15px;
        }
        .quiz-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            font-size: 13px;
            color: #718096;
            margin-bottom: 12px;
        }
        .quiz-meta i {
            margin-right: 3px;
        }
        .quiz-status {
            display: flex;
            gap: 10px;
            margin-bottom: 15px;
            flex-wrap: wrap;
        }
        .quiz-actions {
            display: flex;
            gap: 10px;
        }
        .filter-bar {
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 25px;
            background: white;
            padding: 15px 20px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .filter-group {
            display: flex;
            gap: 10px;
            align-items: center;
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
            width: 100%;
        }
        @media (max-width: 768px) {
            .quiz-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>