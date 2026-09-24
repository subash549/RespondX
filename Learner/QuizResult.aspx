<%@ Page Title="Quiz Results - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuizResult.aspx.cs" Inherits="RespondX.Learner.QuizResult" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link runat="server" href="~/Content/Learner.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Panel ID="pnlResult" runat="server" Visible="false">
            <div class="result-container">
                <div class="result-header <%# Convert.ToBoolean(Eval("IsPassed")) ? "passed" : "failed" %>">
                    <div class="result-icon">
                        <i class="fas <%# Convert.ToBoolean(Eval("IsPassed")) ? "fa-check-circle" : "fa-times-circle" %>"></i>
                    </div>
                    <h1><asp:Label ID="lblResultTitle" runat="server"></asp:Label></h1>
                    <p><asp:Label ID="lblResultMessage" runat="server"></asp:Label></p>
                </div>

                <div class="result-stats">
                    <div class="stat-item">
                        <div class="stat-value"><asp:Label ID="lblScore" runat="server"></asp:Label>%</div>
                        <div class="stat-label">Score</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-value"><asp:Label ID="lblCorrect" runat="server"></asp:Label>/<asp:Label ID="lblTotal" runat="server"></asp:Label></div>
                        <div class="stat-label">Correct Answers</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-value"><asp:Label ID="lblPassingScore" runat="server"></asp:Label>%</div>
                        <div class="stat-label">Passing Score</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-value"><asp:Label ID="lblTimeTaken" runat="server"></asp:Label></div>
                        <div class="stat-label">Time Taken</div>
                    </div>
                </div>

                <div class="result-chart">
                    <canvas id="resultChart" width="200" height="200"></canvas>
                </div>

                <div class="result-details">
                    <div class="card">
                        <div class="card-title">Answer Review</div>
                        <asp:Repeater ID="rptAnswers" runat="server">
                            <ItemTemplate>
                                <div class="answer-review-item <%# Convert.ToBoolean(Eval("IsCorrect")) ? "correct" : "incorrect" %>">
                                    <div class="answer-status">
                                        <i class="fas fa-<%# Convert.ToBoolean(Eval("IsCorrect")) ? "check" : "times" %>"></i>
                                    </div>
                                    <div class="answer-content">
                                        <h4><%# Eval("QuestionText") %></h4>
                                        <p><strong>Your Answer:</strong> <%# Eval("SelectedAnswer") %></p>
                                        <p><strong>Correct Answer:</strong> <%# Eval("CorrectAnswer") %></p>
                                        <%# Convert.ToBoolean(Eval("IsCorrect")) ? "" : "<p class='text-danger'><strong>Explanation:</strong> " + Eval("Explanation") + "</p>" %>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="result-actions">
                    <a href="Quizzes.aspx" class="btn btn-secondary">Back to Quizzes</a>
                    <asp:Button ID="btnRetake" runat="server" Text="Retake Quiz" CssClass="btn btn-primary" OnClick="btnRetake_Click" />
                    <asp:Button ID="btnCertificate" runat="server" Text="View Certificate" CssClass="btn btn-success" OnClick="btnCertificate_Click" Visible="false" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="alert alert-warning">
                <h4>Results Not Found</h4>
                <p>The quiz results you're looking for don't exist.</p>
                <a href="Quizzes.aspx" class="btn btn-primary">Back to Quizzes</a>
            </div>
        </asp:Panel>
    </div>

    <style>
        .result-container {
            background: white;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .result-header {
            text-align: center;
            padding: 30px 20px;
            border-radius: 10px;
            margin-bottom: 30px;
        }
        .result-header.passed {
            background: linear-gradient(135deg, #48bb78, #38a169);
            color: white;
        }
        .result-header.failed {
            background: linear-gradient(135deg, #fc8181, #f56565);
            color: white;
        }
        .result-icon {
            font-size: 60px;
            margin-bottom: 15px;
        }
        .result-header h1 {
            margin: 0;
            font-size: 32px;
        }
        .result-header p {
            margin: 10px 0 0;
            font-size: 16px;
            opacity: 0.9;
        }
        .result-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        .stat-item {
            text-align: center;
            padding: 15px;
            background: #f7fafc;
            border-radius: 8px;
        }
        .stat-value {
            font-size: 28px;
            font-weight: 700;
            color: #2d3748;
        }
        .stat-label {
            font-size: 14px;
            color: #718096;
            margin-top: 5px;
        }
        .result-chart {
            max-width: 300px;
            margin: 0 auto 30px;
        }
        .answer-review-item {
            display: flex;
            gap: 15px;
            padding: 15px 0;
            border-bottom: 1px solid #e2e8f0;
        }
        .answer-review-item:last-child {
            border-bottom: none;
        }
        .answer-review-item.correct {
            border-left: 4px solid #48bb78;
            padding-left: 15px;
        }
        .answer-review-item.incorrect {
            border-left: 4px solid #fc8181;
            padding-left: 15px;
        }
        .answer-status {
            font-size: 20px;
            flex-shrink: 0;
            padding-top: 3px;
        }
        .answer-status .fa-check { color: #48bb78; }
        .answer-status .fa-times { color: #fc8181; }
        .answer-content {
            flex: 1;
        }
        .answer-content h4 {
            margin: 0 0 5px;
            font-size: 15px;
            color: #2d3748;
        }
        .answer-content p {
            margin: 3px 0;
            font-size: 14px;
            color: #4a5568;
        }
        .answer-content .text-danger {
            color: #fc8181;
        }
        .result-actions {
            display: flex;
            gap: 10px;
            justify-content: center;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 2px solid #e2e8f0;
            flex-wrap: wrap;
        }
        @media (max-width: 768px) {
            .result-stats {
                grid-template-columns: repeat(2, 1fr);
            }
        }
    </style>
</asp:Content>