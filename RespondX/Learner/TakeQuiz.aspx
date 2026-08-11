<%@ Page Title="Take Quiz - RespondX" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TakeQuiz.aspx.cs" Inherits="RespondX.Learner.TakeQuiz" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/Content/Learner.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>
<asp:Label ID="lblTotalSeconds" runat="server" Visible="false"></asp:Label>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Panel ID="pnlQuiz" runat="server" Visible="false">
            <div class="quiz-container">
                <div class="quiz-header">
                    <div class="quiz-info">
                        <h1><asp:Label ID="lblQuizTitle" runat="server"></asp:Label></h1>
                        <p><asp:Label ID="lblQuizDescription" runat="server"></asp:Label></p>
                        <div class="quiz-meta">
                            <span><i class="fas fa-clock"></i> Time Limit: <asp:Label ID="lblTimeLimit" runat="server"></asp:Label> min</span>
                            <span><i class="fas fa-question"></i> <asp:Label ID="lblTotalQuestions" runat="server"></asp:Label> questions</span>
                            <span><i class="fas fa-check-circle"></i> Passing: <asp:Label ID="lblPassingScore" runat="server"></asp:Label>%</span>
                        </div>
                    </div>
                    <div class="quiz-timer" data-total-time='<asp:Label ID="lblTotalSeconds" runat="server"></asp:Label>'>
                        <div class="timer-display">00:00</div>
                        <div class="timer-progress">
                            <div class="timer-progress-bar" style="width: 0%"></div>
                        </div>
                        <span class="timer-label">Time Remaining</span>
                    </div>
                </div>

                <div class="quiz-progress-header">
                    <div class="question-counter">1 of <asp:Label ID="Label1" runat="server"></asp:Label></div>
                    <div class="progress">
                        <div class="quiz-progress-bar" style="width: 0%"></div>
                    </div>
                    <div class="question-nav">
                        <asp:Repeater ID="rptQuestionNav" runat="server">
                            <ItemTemplate>
                                <span class="question-nav-item unanswered" data-index='<%# Container.ItemIndex %>' data-questionid='<%# Eval("QuestionID") %>'>
                                    <%# Container.ItemIndex + 1 %>
                                </span>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="quiz-body">
                    <asp:Repeater ID="rptQuestions" runat="server">
                        <ItemTemplate>
                            <div class="question-card" data-index='<%# Container.ItemIndex %>' data-questionid='<%# Eval("QuestionID") %>' style='display: <%# Container.ItemIndex == 0 ? "block" : "none" %>;'>
                                <div class="question-header">
                                    <span class="question-number">Question <%# Container.ItemIndex + 1 %> of <%# ((Repeater)Container.NamingContainer).Items.Count %></span>
                                    <span class="question-points">Points: <%# Eval("Points") %></span>
                                </div>
                                <div class="question-text">
                                    <%# Eval("QuestionText") %>
                                </div>
                                <div class="answer-options">
                                    <asp:Repeater ID="rptOptions" runat="server" DataSource='<%# Eval("Options") %>'>
                                        <ItemTemplate>
                                            <div class="answer-option">
                                                <input type="radio" name="question_<%# Eval("QuestionID") %>" value='<%# Eval("OptionID") %>' id='opt_<%# Eval("OptionID") %>' />
                                                <label for='opt_<%# Eval("OptionID") %>'><%# Eval("OptionLabel") %>. <%# Eval("OptionText") %></label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="quiz-navigation">
                    <button type="button" class="btn btn-secondary btn-prev" disabled>Previous</button>
                    <button type="button" class="btn btn-primary btn-next">Next</button>
                    <asp:Button ID="btnSubmitQuiz" runat="server" Text="Submit Quiz" CssClass="btn btn-success" OnClick="btnSubmitQuiz_Click" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="alert alert-warning">
                <h4>Quiz Not Found</h4>
                <p>The quiz you're looking for doesn't exist or is not available.</p>
                <a href="Quizzes.aspx" class="btn btn-primary">Back to Quizzes</a>
            </div>
        </asp:Panel>
    </div>

    <style>
        .quiz-container {
            background: white;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
        }
        .quiz-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 25px;
            padding-bottom: 20px;
            border-bottom: 2px solid #e2e8f0;
        }
        .quiz-info h1 {
            margin: 0 0 5px;
        }
        .quiz-info p {
            color: #718096;
            margin-bottom: 10px;
        }
        .quiz-meta {
            display: flex;
            gap: 15px;
            font-size: 14px;
            color: #718096;
        }
        .quiz-timer {
            text-align: center;
            min-width: 120px;
        }
        .timer-display {
            font-size: 32px;
            font-weight: 700;
            color: #667eea;
        }
        .quiz-timer.warning .timer-display {
            color: #fc8181;
            animation: pulse 1s infinite;
        }
        @keyframes pulse {
            0%, 100% { opacity: 1; }
            50% { opacity: 0.5; }
        }
        .timer-progress {
            height: 4px;
            background: #e2e8f0;
            border-radius: 2px;
            margin: 5px 0;
            overflow: hidden;
        }
        .timer-progress-bar {
            height: 100%;
            background: #667eea;
            transition: width 1s linear;
        }
        .timer-label {
            font-size: 12px;
            color: #a0aec0;
        }
        .quiz-progress-header {
            margin-bottom: 25px;
        }
        .question-counter {
            text-align: center;
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 5px;
        }
        .question-nav {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            margin-top: 10px;
            justify-content: center;
        }
        .question-nav-item {
            width: 35px;
            height: 35px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            border: 2px solid #e2e8f0;
            transition: all 0.3s;
        }
        .question-nav-item:hover {
            border-color: #667eea;
        }
        .question-nav-item.answered {
            background: #48bb78;
            color: white;
            border-color: #48bb78;
        }
        .question-nav-item.unanswered {
            background: #fc8181;
            color: white;
            border-color: #fc8181;
        }
        .question-nav-item.active {
            border-color: #667eea;
            background: #667eea;
            color: white;
        }
        .question-card {
            display: none;
            padding: 20px 0;
        }
        .question-header {
            display: flex;
            justify-content: space-between;
            margin-bottom: 15px;
        }
        .question-number {
            font-weight: 600;
            color: #667eea;
        }
        .question-points {
            color: #718096;
            font-size: 14px;
        }
        .question-text {
            font-size: 18px;
            color: #2d3748;
            margin-bottom: 20px;
            line-height: 1.6;
        }
        .answer-options {
            display: grid;
            gap: 12px;
        }
        .answer-option {
            display: flex;
            align-items: center;
            padding: 12px 15px;
            border: 2px solid #e2e8f0;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.3s;
        }
        .answer-option:hover {
            border-color: #667eea;
            background: #f7fafc;
        }
        .answer-option.selected {
            border-color: #667eea;
            background: #ebf4ff;
        }
        .answer-option input[type="radio"] {
            width: 18px;
            height: 18px;
            margin-right: 12px;
            cursor: pointer;
        }
        .answer-option label {
            cursor: pointer;
            margin: 0;
            flex: 1;
            font-weight: 500;
        }
        .quiz-navigation {
            display: flex;
            justify-content: center;
            gap: 10px;
            margin-top: 25px;
            padding-top: 20px;
            border-top: 2px solid #e2e8f0;
        }
        @media (max-width: 768px) {
            .quiz-header {
                flex-direction: column;
                gap: 15px;
            }
            .quiz-timer {
                align-self: center;
            }
            .question-nav-item {
                width: 30px;
                height: 30px;
                font-size: 12px;
            }
        }
    </style>

    <script>
        $(document).ready(function() {
            // Initialize quiz timer
            var totalSeconds = parseInt($('.quiz-timer').data('total-time'));
            if (totalSeconds > 0) {
                window.quizTimer = new QuizTimer(totalSeconds);
            }

            // Initialize quiz navigation
            window.quizNavigator = new QuizNavigator($('.question-card').length);
        });

        class QuizTimer {
            constructor(totalSeconds) {
                this.totalSeconds = totalSeconds;
                this.timeRemaining = totalSeconds;
                this.timerInterval = null;
                this.isPaused = false;
                this.startTimer();
            }

            startTimer() {
                this.timerInterval = setInterval(() => {
                    if (!this.isPaused) {
                        this.timeRemaining--;
                        this.updateDisplay();
                        
                        if (this.timeRemaining <= 0) {
                            this.stopTimer();
                            this.timeUp();
                        }
                    }
                }, 1000);
            }

            stopTimer() {
                if (this.timerInterval) {
                    clearInterval(this.timerInterval);
                    this.timerInterval = null;
                }
            }

            pauseTimer() {
                this.isPaused = true;
            }

            resumeTimer() {
                this.isPaused = false;
            }

            updateDisplay() {
                var minutes = Math.floor(this.timeRemaining / 60);
                var seconds = this.timeRemaining % 60;
                var display = this.padZero(minutes) + ':' + this.padZero(seconds);
                $('.timer-display').text(display);
                
                if (this.timeRemaining < 60) {
                    $('.quiz-timer').addClass('warning');
                }
                
                var progress = ((this.totalSeconds - this.timeRemaining) / this.totalSeconds) * 100;
                $('.timer-progress-bar').css('width', progress + '%');
            }

            padZero(num) {
                return (num < 10) ? '0' + num : num;
            }

            timeUp() {
                alert('Time is up! Your quiz will be submitted automatically.');
                $('#btnSubmitQuiz').click();
            }

            getTimeRemaining() {
                return this.timeRemaining;
            }
        }

        class QuizNavigator {
            constructor(totalQuestions) {
                this.currentQuestion = 0;
                this.totalQuestions = totalQuestions;
                this.answers = {};
                this.init();
            }

            init() {
                this.updateNavigation();
                this.updateProgress();
                this.setupEventListeners();
            }

            setupEventListeners() {
                $('.btn-prev').click(() => this.prevQuestion());
                $('.btn-next').click(() => this.nextQuestion());
                $('.question-nav-item').click((e) => {
                    var index = $(e.target).data('index');
                    this.goToQuestion(index);
                });
                $('.answer-option input[type="radio"]').change((e) => {
                    this.saveAnswer(e.target);
                });
            }

            showQuestion(index) {
                if (index < 0 || index >= this.totalQuestions) return;
                this.currentQuestion = index;
                $('.question-card').hide();
                $('.question-card[data-index="' + index + '"]').show();
                this.updateNavigation();
                this.updateProgress();
                this.updateNavItems();
            }

            nextQuestion() {
                if (this.currentQuestion < this.totalQuestions - 1) {
                    this.showQuestion(this.currentQuestion + 1);
                }
            }

            prevQuestion() {
                if (this.currentQuestion > 0) {
                    this.showQuestion(this.currentQuestion - 1);
                }
            }

            goToQuestion(index) {
                if (index >= 0 && index < this.totalQuestions) {
                    this.showQuestion(index);
                }
            }

            saveAnswer(input) {
                var questionId = $(input).closest('.question-card').data('questionid');
                var value = $(input).val();
                this.answers[questionId] = value;
                this.updateNavItems();
            }

            updateNavigation() {
                $('.btn-prev').prop('disabled', this.currentQuestion === 0);
                $('.btn-next').prop('disabled', this.currentQuestion === this.totalQuestions - 1);
                $('.question-counter').text((this.currentQuestion + 1) + ' of ' + this.totalQuestions);
            }

            updateProgress() {
                var progress = ((this.currentQuestion + 1) / this.totalQuestions) * 100;
                $('.quiz-progress-bar').css('width', progress + '%');
            }

            updateNavItems() {
                $('.question-nav-item').each((index, item) => {
                    var questionId = $(item).data('questionid');
                    if (this.answers[questionId]) {
                        $(item).removeClass('unanswered').addClass('answered');
                    } else {
                        $(item).removeClass('answered').addClass('unanswered');
                    }
                    if (index === this.currentQuestion) {
                        $(item).addClass('active');
                    } else {
                        $(item).removeClass('active');
                    }
                });
            }

            getAnswers() {
                return this.answers;
            }
        }
    </script>
</asp:Content>