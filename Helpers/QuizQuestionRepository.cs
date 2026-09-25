using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using RespondX.Models;

namespace RespondX.Helpers
{
    public static class QuizQuestionRepository
    {
        private const int LearnerQuestionIdOffset = 1000000;

        private static string ConnectionString => DatabaseHelper.ConnectionString;

        public static List<QuestionItem> GetActiveQuestions(int quizId)
        {
            try
            {
                return ReadQuestionsForDatabaseQuiz(quizId);
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load quiz questions", ex.Message, ex.StackTrace);
                return new List<QuestionItem>();
            }
        }

        public static List<QuestionItem> GetActiveQuestionsForQuiz(int quizId)
        {
            try
            {
                return ReadQuestionsForDatabaseQuiz(quizId);
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Load learner quiz questions", ex.Message, ex.StackTrace);
                return new List<QuestionItem>();
            }
        }

        public static List<QuestionItem> GetQuestionsForAdmin(int quizId)
        {
            return ReadQuestionsForDatabaseQuiz(quizId);
        }

        public static int GetActiveQuestionCount(int quizId)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.Questions WHERE QuizID = @QuizID AND QuestionType = N'MultipleChoice'", connection))
                {
                    command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Count quiz questions", ex.Message, ex.StackTrace);
                return 0;
            }
        }

        public static void AddQuestion(int quizId, string questionText, int points,
            string[] optionTexts, int correctOptionIndex)
        {
            ValidateQuestion(questionText, points, optionTexts, correctOptionIndex);

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        EnsureQuizExists(connection, transaction, quizId);

                        int questionOrder;
                        using (var orderCommand = new SqlCommand(@"
                            SELECT ISNULL(MAX(QuestionOrder), 0) + 1
                            FROM dbo.Questions WITH (UPDLOCK, HOLDLOCK)
                            WHERE QuizID = @QuizID;", connection, transaction))
                        {
                            orderCommand.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;
                            questionOrder = Convert.ToInt32(orderCommand.ExecuteScalar());
                        }

                        int questionId;
                        using (var questionCommand = new SqlCommand(@"
                            INSERT INTO dbo.Questions
                                (QuizID, QuestionText, QuestionType, DifficultyLevel, Points, QuestionOrder)
                            VALUES
                                (@QuizID, @QuestionText, N'MultipleChoice', 1, @Points, @QuestionOrder);
                            SELECT CONVERT(INT, SCOPE_IDENTITY());", connection, transaction))
                        {
                            questionCommand.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;
                            questionCommand.Parameters.Add("@QuestionText", SqlDbType.NVarChar, -1).Value = questionText.Trim();
                            questionCommand.Parameters.Add("@Points", SqlDbType.Int).Value = points;
                            questionCommand.Parameters.Add("@QuestionOrder", SqlDbType.Int).Value = questionOrder;
                            questionId = Convert.ToInt32(questionCommand.ExecuteScalar());
                        }

                        for (int index = 0; index < optionTexts.Length; index++)
                        {
                            using (var optionCommand = new SqlCommand(@"
                                INSERT INTO dbo.QuizOptions (QuestionID, OptionText, IsCorrect, OptionOrder)
                                VALUES (@QuestionID, @OptionText, @IsCorrect, @OptionOrder);", connection, transaction))
                            {
                                optionCommand.Parameters.Add("@QuestionID", SqlDbType.Int).Value = questionId;
                                optionCommand.Parameters.Add("@OptionText", SqlDbType.NVarChar, -1).Value = optionTexts[index].Trim();
                                optionCommand.Parameters.Add("@IsCorrect", SqlDbType.Bit).Value = index == correctOptionIndex;
                                optionCommand.Parameters.Add("@OptionOrder", SqlDbType.Int).Value = index + 1;
                                optionCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void UpdateQuestion(int quizId, int learnerQuestionId, string questionText,
            int points, string[] optionTexts, int correctOptionIndex)
        {
            ValidateQuestion(questionText, points, optionTexts, correctOptionIndex);

            int databaseQuestionId = learnerQuestionId - LearnerQuestionIdOffset;
            if (databaseQuestionId <= 0)
                throw new InvalidOperationException("Select a saved question to update.");

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        EnsureQuizExists(connection, transaction, quizId);

                        using (var updateQuestion = new SqlCommand(@"
                            UPDATE dbo.Questions
                            SET QuestionText = @QuestionText, Points = @Points
                            WHERE QuestionID = @QuestionID AND QuizID = @QuizID
                              AND QuestionType = N'MultipleChoice';", connection, transaction))
                        {
                            updateQuestion.Parameters.Add("@QuestionText", SqlDbType.NVarChar, -1).Value = questionText.Trim();
                            updateQuestion.Parameters.Add("@Points", SqlDbType.Int).Value = points;
                            updateQuestion.Parameters.Add("@QuestionID", SqlDbType.Int).Value = databaseQuestionId;
                            updateQuestion.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;
                            if (updateQuestion.ExecuteNonQuery() != 1)
                                throw new InvalidOperationException("That question does not belong to the selected quiz.");
                        }

                        var optionIds = new List<int>();
                        using (var selectOptions = new SqlCommand(@"
                            SELECT OptionID
                            FROM dbo.QuizOptions WITH (UPDLOCK, HOLDLOCK)
                            WHERE QuestionID = @QuestionID
                            ORDER BY OptionOrder, OptionID;", connection, transaction))
                        {
                            selectOptions.Parameters.Add("@QuestionID", SqlDbType.Int).Value = databaseQuestionId;
                            using (var reader = selectOptions.ExecuteReader())
                            {
                                while (reader.Read())
                                    optionIds.Add(Convert.ToInt32(reader["OptionID"]));
                            }
                        }

                        if (optionIds.Count != 4)
                            throw new InvalidOperationException("This saved question does not have four choices and cannot be edited here.");

                        for (int index = 0; index < optionIds.Count; index++)
                        {
                            using (var updateOption = new SqlCommand(@"
                                UPDATE dbo.QuizOptions
                                SET OptionText = @OptionText, IsCorrect = @IsCorrect, OptionOrder = @OptionOrder
                                WHERE OptionID = @OptionID AND QuestionID = @QuestionID;", connection, transaction))
                            {
                                updateOption.Parameters.Add("@OptionText", SqlDbType.NVarChar, -1).Value = optionTexts[index].Trim();
                                updateOption.Parameters.Add("@IsCorrect", SqlDbType.Bit).Value = index == correctOptionIndex;
                                updateOption.Parameters.Add("@OptionOrder", SqlDbType.Int).Value = index + 1;
                                updateOption.Parameters.Add("@OptionID", SqlDbType.Int).Value = optionIds[index];
                                updateOption.Parameters.Add("@QuestionID", SqlDbType.Int).Value = databaseQuestionId;
                                updateOption.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static void ValidateQuestion(string questionText, int points, string[] optionTexts, int correctOptionIndex)
        {
            if (string.IsNullOrWhiteSpace(questionText))
                throw new InvalidOperationException("Question text is required.");
            if (points < 1 || points > 100)
                throw new InvalidOperationException("Points must be between 1 and 100.");
            if (optionTexts == null || optionTexts.Length != 4)
                throw new InvalidOperationException("A multiple-choice question must have four options.");
            if (correctOptionIndex < 0 || correctOptionIndex >= optionTexts.Length)
                throw new InvalidOperationException("Choose exactly one correct answer.");
            foreach (string option in optionTexts)
            {
                if (string.IsNullOrWhiteSpace(option))
                    throw new InvalidOperationException("Fill in all four answer choices.");
            }
        }

        private static List<QuestionItem> ReadQuestionsForDatabaseQuiz(int quizId)
        {
            if (quizId <= 0)
                return new List<QuestionItem>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                return ReadQuestions(connection, quizId);
            }
        }

        private static List<QuestionItem> ReadQuestions(SqlConnection connection, int databaseQuizId)
        {
            var questions = new List<QuestionItem>();
            var byId = new Dictionary<int, QuestionItem>();
            using (var command = new SqlCommand(@"
                    SELECT q.QuestionID, q.QuestionText, q.QuestionType, q.Points,
                           o.OptionID, o.OptionText, o.IsCorrect, o.OptionOrder
                    FROM dbo.Questions q
                    LEFT JOIN dbo.QuizOptions o ON o.QuestionID = q.QuestionID
                    WHERE q.QuizID = @QuizID AND q.QuestionType = N'MultipleChoice'
                    ORDER BY q.QuestionOrder, q.QuestionID, o.OptionOrder, o.OptionID;", connection))
            {
                command.Parameters.Add("@QuizID", SqlDbType.Int).Value = databaseQuizId;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int databaseQuestionId = Convert.ToInt32(reader["QuestionID"]);
                        QuestionItem question;
                        if (!byId.TryGetValue(databaseQuestionId, out question))
                        {
                            string questionType = Convert.ToString(reader["QuestionType"]);
                            question = new QuestionItem
                            {
                                QuestionID = LearnerQuestionIdOffset + databaseQuestionId,
                                QuestionText = Convert.ToString(reader["QuestionText"]),
                                QuestionType = questionType,
                                QuestionTypeDisplay = questionType == "MultipleChoice" ? "Multiple Choice" : questionType,
                                Points = Convert.ToInt32(reader["Points"]),
                                IsActive = true,
                                Options = new List<OptionItem>()
                            };
                            byId.Add(databaseQuestionId, question);
                            questions.Add(question);
                        }

                        if (reader["OptionID"] != DBNull.Value)
                        {
                            int optionOrder = Convert.ToInt32(reader["OptionOrder"]);
                            question.Options.Add(new OptionItem
                            {
                                QuestionID = question.QuestionID,
                                OptionID = LearnerQuestionIdOffset + Convert.ToInt32(reader["OptionID"]),
                                OptionText = Convert.ToString(reader["OptionText"]),
                                OptionLabel = ((char)('A' + optionOrder - 1)).ToString(),
                                IsCorrect = Convert.ToBoolean(reader["IsCorrect"]),
                                OptionOrder = optionOrder
                            });
                        }
                    }
                }
            }

            return questions;
        }

        private static void EnsureQuizExists(SqlConnection connection, SqlTransaction transaction, int quizId)
        {
            if (quizId <= 0)
                throw new InvalidOperationException("Select a valid quiz.");

            using (var command = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Quizzes WHERE QuizID = @QuizID;", connection, transaction))
            {
                command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizId;
                if (Convert.ToInt32(command.ExecuteScalar()) != 1)
                    throw new InvalidOperationException("The quiz record could not be found.");
            }
        }
    }
}
