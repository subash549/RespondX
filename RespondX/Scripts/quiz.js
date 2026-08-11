// ========== Quiz JavaScript ==========

// Quiz Timer
var QuizTimer = {
    timerInterval: null,
    timeRemaining: 0,
    totalTime: 0,
    isPaused: false,
    callbacks: {},

    init: function (totalSeconds, onComplete) {
        this.totalTime = totalSeconds;
        this.timeRemaining = totalSeconds;
        this.callbacks.onComplete = onComplete || function () { };

        this.updateDisplay();
        this.startTimer();
    },

    startTimer: function () {
        var self = this;
        this.timerInterval = setInterval(function () {
            if (!self.isPaused) {
                self.timeRemaining--;
                self.updateDisplay();

                if (self.timeRemaining <= 0) {
                    self.stopTimer();
                    self.callbacks.onComplete();
                }
            }
        }, 1000);
    },

    stopTimer: function () {
        if (this.timerInterval) {
            clearInterval(this.timerInterval);
            this.timerInterval = null;
        }
    },

    pauseTimer: function () {
        this.isPaused = true;
    },

    resumeTimer: function () {
        this.isPaused = false;
    },

    updateDisplay: function () {
        var minutes = Math.floor(this.timeRemaining / 60);
        var seconds = this.timeRemaining % 60;
        var display = this.padZero(minutes) + ':' + this.padZero(seconds);

        $('.quiz-timer .timer-display').text(display);

        // Add warning when time is running low
        if (this.timeRemaining < 60) {
            $('.quiz-timer').addClass('warning');
        } else {
            $('.quiz-timer').removeClass('warning');
        }

        // Update progress bar
        var progress = ((this.totalTime - this.timeRemaining) / this.totalTime) * 100;
        $('.timer-progress-bar').css('width', progress + '%');
    },

    padZero: function (num) {
        return (num < 10) ? '0' + num : num;
    },

    getTimeRemaining: function () {
        return this.timeRemaining;
    }
};

// ========== Quiz Navigation ==========
var QuizNavigator = {
    currentQuestion: 0,
    totalQuestions: 0,
    answers: {},
    questionStatus: {},

    init: function (totalQuestions) {
        this.totalQuestions = totalQuestions;
        this.currentQuestion = 0;
        this.answers = {};
        this.questionStatus = {};

        this.updateNavigation();
        this.showQuestion(0);
    },

    showQuestion: function (index) {
        if (index < 0 || index >= this.totalQuestions) return;

        this.currentQuestion = index;

        // Hide all questions
        $('.question-card').hide();

        // Show current question
        $('.question-card[data-index="' + index + '"]').show();

        // Update navigation
        this.updateNavigation();
        this.updateProgress();
    },

    nextQuestion: function () {
        if (this.currentQuestion < this.totalQuestions - 1) {
            this.saveCurrentAnswer();
            this.showQuestion(this.currentQuestion + 1);
        }
    },

    prevQuestion: function () {
        if (this.currentQuestion > 0) {
            this.saveCurrentAnswer();
            this.showQuestion(this.currentQuestion - 1);
        }
    },

    goToQuestion: function (index) {
        if (index >= 0 && index < this.totalQuestions) {
            this.saveCurrentAnswer();
            this.showQuestion(index);
        }
    },

    saveCurrentAnswer: function () {
        var currentQuestion = $('.question-card[data-index="' + this.currentQuestion + '"]');
        var questionId = currentQuestion.data('questionid');
        var selectedOption = currentQuestion.find('input:checked');

        if (selectedOption.length > 0) {
            this.answers[questionId] = selectedOption.val();
            this.questionStatus[questionId] = 'answered';
        } else {
            if (this.answers[questionId]) {
                delete this.answers[questionId];
                this.questionStatus[questionId] = 'unanswered';
            }
        }

        this.updateQuestionStatus();
    },

    updateNavigation: function () {
        // Update button states
        $('.btn-prev').prop('disabled', this.currentQuestion === 0);
        $('.btn-next').prop('disabled', this.currentQuestion === this.totalQuestions - 1);

        // Update question counter
        $('.question-counter').text((this.currentQuestion + 1) + ' of ' + this.totalQuestions);
    },

    updateProgress: function () {
        var progress = ((this.currentQuestion + 1) / this.totalQuestions) * 100;
        $('.quiz-progress-bar').css('width', progress + '%');
    },

    updateQuestionStatus: function () {
        $('.question-nav-item').each(function () {
            var questionId = $(this).data('questionid');
            var status = QuizNavigator.questionStatus[questionId];

            $(this).removeClass('answered unanswered');
            if (status === 'answered') {
                $(this).addClass('answered');
            } else {
                $(this).addClass('unanswered');
            }
        });
    },

    getAnswers: function () {
        this.saveCurrentAnswer();
        return this.answers;
    },

    isAllAnswered: function () {
        var answeredCount = Object.keys(this.questionStatus).filter(
            function (key) { return this.questionStatus[key] === 'answered'; }, this
        ).length;
        return answeredCount === this.totalQuestions;
    }
};

// ========== Quiz Submission ==========
var QuizSubmitter = {
    submitting: false,

    submitQuiz: function (quizId, answers) {
        if (this.submitting) return;

        var self = this;
        var unanswered = this.getUnansweredQuestions();

        if (unanswered.length > 0) {
            if (!confirm('You have ' + unanswered.length + ' unanswered questions. Are you sure you want to submit?')) {
                return;
            }
        }

        this.submitting = true;
        $('#btn-submit-quiz').prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Submitting...');

        $.ajax({
            url: '/Learner/SubmitQuiz',
            method: 'POST',
            data: {
                quizId: quizId,
                answers: answers
            },
            success: function (response) {
                if (response.success) {
                    window.location.href = '/Learner/QuizResult.aspx?id=' + response.attemptId;
                } else {
                    showNotification(response.message || 'Failed to submit quiz', 'error');
                    self.submitting = false;
                    $('#btn-submit-quiz').prop('disabled', false).html('Submit Quiz');
                }
            },
            error: function () {
                showNotification('An error occurred while submitting the quiz', 'error');
                self.submitting = false;
                $('#btn-submit-quiz').prop('disabled', false).html('Submit Quiz');
            }
        });
    },

    getUnansweredQuestions: function () {
        var unanswered = [];
        $('.question-card').each(function () {
            var questionId = $(this).data('questionid');
            var isAnswered = $(this).find('input:checked').length > 0;
            if (!isAnswered) {
                unanswered.push(questionId);
            }
        });
        return unanswered;
    }
};

// ========== Quiz Review ==========
var QuizReview = {
    init: function () {
        $('.review-answer').each(function () {
            var isCorrect = $(this).data('correct');
            if (isCorrect) {
                $(this).addClass('correct-answer');
            } else {
                $(this).addClass('incorrect-answer');
            }
        });
    }
};

// ========== Quiz Document Ready ==========
$(document).ready(function () {
    // Initialize quiz timer if timer element exists
    if ($('.quiz-timer').length > 0) {
        var totalTime = parseInt($('.quiz-timer').data('total-time'));
        QuizTimer.init(totalTime, function () {
            // Time's up - auto submit
            showNotification('Time is up! Submitting your quiz...', 'warning');
            QuizSubmitter.submitQuiz(
                $('.quiz-container').data('quiz-id'),
                QuizNavigator.getAnswers()
            );
        });
    }

    // Initialize quiz navigation
    if ($('.question-card').length > 0) {
        var totalQuestions = $('.question-card').length;
        QuizNavigator.init(totalQuestions);
    }

    // Handle answer selection
    $(document).on('change', '.answer-option input', function () {
        QuizNavigator.saveCurrentAnswer();
    });

    // Handle navigation buttons
    $('.btn-prev').click(function () {
        QuizNavigator.prevQuestion();
    });

    $('.btn-next').click(function () {
        QuizNavigator.nextQuestion();
    });

    // Handle question navigation dots
    $('.question-nav-item').click(function () {
        var index = $(this).data('index');
        QuizNavigator.goToQuestion(index);
    });

    // Handle quiz submission
    $('#btn-submit-quiz').click(function () {
        var quizId = $(this).data('quiz-id');
        var answers = QuizNavigator.getAnswers();
        QuizSubmitter.submitQuiz(quizId, answers);
    });

    // Initialize review page
    if ($('.quiz-review').length > 0) {
        QuizReview.init();
    }
});