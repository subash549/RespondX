using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace RespondX.Helpers
{
    public static class QuizAccessHelper
    {
        private static string ConnectionString => DatabaseHelper.ConnectionString;

        public static int GetModuleIdByTitle(string moduleTitle)
        {
            if (string.IsNullOrWhiteSpace(moduleTitle))
                return 0;

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 ModuleID
                    FROM Modules
                    WHERE IsActive = 1
                      AND LOWER(LTRIM(RTRIM(Title))) = LOWER(LTRIM(RTRIM(@Title)))
                    ORDER BY ModuleID;", conn))
                {
                    cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = moduleTitle.Trim();
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Get quiz module", ex.Message, ex.StackTrace);
                return 0;
            }
        }

        public static bool IsModuleCompleted(int moduleId, int learnerId)
        {
            if (moduleId <= 0 || learnerId <= 0)
                return false;

            return CheckModuleCompletion("m.ModuleID = @ModuleID", moduleId, null, learnerId);
        }

        public static bool IsModuleCompleted(string moduleTitle, int learnerId)
        {
            if (string.IsNullOrWhiteSpace(moduleTitle) || learnerId <= 0)
                return false;

            return CheckModuleCompletion(
                "LOWER(LTRIM(RTRIM(m.Title))) = LOWER(LTRIM(RTRIM(@ModuleTitle)))",
                null,
                moduleTitle.Trim(),
                learnerId);
        }

        private static bool CheckModuleCompletion(string modulePredicate, int? moduleId, string moduleTitle, int learnerId)
        {
            var query = @"
                SELECT CASE WHEN EXISTS
                (
                    SELECT 1
                    FROM Modules m
                    WHERE m.IsActive = 1
                      AND " + modulePredicate + @"
                      AND EXISTS
                      (
                          SELECT 1 FROM Lessons l
                          WHERE l.ModuleID = m.ModuleID AND l.IsActive = 1
                      )
                      AND NOT EXISTS
                      (
                          SELECT 1
                          FROM Lessons l
                          WHERE l.ModuleID = m.ModuleID
                            AND l.IsActive = 1
                            AND NOT EXISTS
                            (
                                SELECT 1
                                FROM LearnerProgress lp
                                WHERE lp.LessonID = l.LessonID
                                  AND lp.LearnerID = @LearnerID
                                  AND lp.Status IN (N'Completed', N'Certified')
                            )
                      )
                ) THEN 1 ELSE 0 END;";

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (moduleId.HasValue)
                        cmd.Parameters.Add("@ModuleID", SqlDbType.Int).Value = moduleId.Value;
                    else
                        cmd.Parameters.Add("@ModuleTitle", SqlDbType.NVarChar, 100).Value = moduleTitle;

                    cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId;
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
                }
            }
            catch (Exception ex)
            {
                // A database issue must never grant access to a quiz.
                DatabaseHelper.LogError("Check quiz module completion", ex.Message, ex.StackTrace);
                return false;
            }
        }
    }
}
