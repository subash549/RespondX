using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RespondX.Models;

namespace RespondX.Helpers
{
    /// <summary>
    /// Alerts are stored one row per learner so each learner has their own read/acknowledged state.
    /// An alert sent to "all learners" is therefore a group of rows created by one INSERT; the group is
    /// identified by its creation time, sender, title, type and priority, and represented by its lowest AlertID.
    /// </summary>
    public static class AlertRepository
    {
        private const string GroupJoin = @"
            INNER JOIN dbo.Alerts r ON r.AlertID = @AlertID
            WHERE a.CreatedAt = r.CreatedAt
              AND ISNULL(a.AdminID, 0) = ISNULL(r.AdminID, 0)
              AND a.Title = r.Title
              AND a.AlertType = r.AlertType
              AND a.Priority = r.Priority";

        public static readonly string[] AlertTypes = { "Emergency", "Reminder", "Notification", "System" };

        // ========== Admin ==========

        public static List<AlertItem> GetAlertGroups(string alertType, string status)
        {
            var alerts = new List<AlertItem>();
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT g.AlertID, g.RecipientCount, g.ReadCount,
                       a.Title, a.Message, a.AlertType, a.Priority, a.CreatedAt, a.ExpiresAt,
                       u.FirstName + N' ' + u.LastName AS LearnerName
                FROM (
                    SELECT MIN(AlertID) AS AlertID, COUNT(*) AS RecipientCount,
                           SUM(CASE WHEN IsRead = 1 THEN 1 ELSE 0 END) AS ReadCount
                    FROM dbo.Alerts
                    GROUP BY CreatedAt, ISNULL(AdminID, 0), Title, AlertType, Priority
                ) g
                INNER JOIN dbo.Alerts a ON a.AlertID = g.AlertID
                LEFT JOIN dbo.Users u ON u.UserID = a.LearnerID
                WHERE (@Type IS NULL OR a.AlertType = @Type)
                  AND (@Status IS NULL
                       OR (@Status = N'Active' AND (a.ExpiresAt IS NULL OR a.ExpiresAt > GETDATE()))
                       OR (@Status = N'Expired' AND a.ExpiresAt <= GETDATE()))
                ORDER BY a.CreatedAt DESC, g.AlertID DESC;", conn))
            {
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(alertType) ? (object)DBNull.Value : alertType;
                cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = string.IsNullOrEmpty(status) ? (object)DBNull.Value : status;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = ReadAlert(reader);
                        item.RecipientCount = Convert.ToInt32(reader["RecipientCount"]);
                        item.ReadCount = Convert.ToInt32(reader["ReadCount"]);
                        item.Target = item.RecipientCount == 1 && reader["LearnerName"] != DBNull.Value
                            ? Convert.ToString(reader["LearnerName"])
                            : item.RecipientCount + " learners";
                        alerts.Add(item);
                    }
                }
            }
            return alerts;
        }

        public static AlertItem GetAlert(int alertId)
        {
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT AlertID, Title, Message, AlertType, Priority, CreatedAt, ExpiresAt, IsRead, IsAcknowledged
                FROM dbo.Alerts WHERE AlertID = @AlertID;", conn))
            {
                cmd.Parameters.Add("@AlertID", SqlDbType.Int).Value = alertId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? ReadAlert(reader) : null;
                }
            }
        }

        /// <summary>Creates the alert for one learner, or for every active learner when learnerId is null. Returns the number of recipients.</summary>
        public static int CreateAlert(string title, string message, string alertType, int priority, DateTime? expiresAt, int? learnerId, int adminId)
        {
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                INSERT INTO dbo.Alerts (LearnerID, AdminID, AlertType, Title, Message, Priority, IsRead, IsAcknowledged, CreatedAt, ExpiresAt)
                SELECT u.UserID, @AdminID, @AlertType, @Title, @Message, @Priority, 0, 0, GETDATE(), @ExpiresAt
                FROM dbo.Users u
                WHERE u.Role = N'Learner' AND u.IsActive = 1
                  AND (@LearnerID IS NULL OR u.UserID = @LearnerID);", conn))
            {
                AddContentParameters(cmd, title, message, alertType, priority, expiresAt);
                cmd.Parameters.Add("@AdminID", SqlDbType.Int).Value = adminId > 0 ? (object)adminId : DBNull.Value;
                cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId.HasValue ? (object)learnerId.Value : DBNull.Value;
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateAlertGroup(int alertId, string title, string message, string alertType, int priority, DateTime? expiresAt, bool changeExpiry)
        {
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                UPDATE a
                SET Title = @Title, Message = @Message, AlertType = @AlertType, Priority = @Priority,
                    ExpiresAt = CASE WHEN @ChangeExpiry = 1 THEN @ExpiresAt ELSE a.ExpiresAt END
                FROM dbo.Alerts a" + GroupJoin + ";", conn))
            {
                AddContentParameters(cmd, title, message, alertType, priority, expiresAt);
                cmd.Parameters.Add("@ChangeExpiry", SqlDbType.Bit).Value = changeExpiry;
                cmd.Parameters.Add("@AlertID", SqlDbType.Int).Value = alertId;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteAlertGroup(int alertId)
        {
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand("DELETE a FROM dbo.Alerts a" + GroupJoin + ";", conn))
            {
                cmd.Parameters.Add("@AlertID", SqlDbType.Int).Value = alertId;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static List<ListItemData> GetActiveLearners()
        {
            var learners = new List<ListItemData>();
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT UserID, FirstName + N' ' + LastName + N' (' + Username + N')' AS DisplayName
                FROM dbo.Users WHERE Role = N'Learner' AND IsActive = 1
                ORDER BY FirstName, LastName;", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        learners.Add(new ListItemData { Value = reader["UserID"].ToString(), Text = reader["DisplayName"].ToString() });
                }
            }
            return learners;
        }

        public sealed class ListItemData
        {
            public string Value;
            public string Text;
        }

        // ========== Learner ==========

        public static List<AlertItem> GetLearnerAlerts(int learnerId, int? top = null)
        {
            var alerts = new List<AlertItem>();
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT TOP (@Top) AlertID, Title, Message, AlertType, Priority, CreatedAt, ExpiresAt, IsRead, IsAcknowledged
                FROM dbo.Alerts
                WHERE LearnerID = @LearnerID AND (ExpiresAt IS NULL OR ExpiresAt > GETDATE())
                ORDER BY IsRead, Priority DESC, CreatedAt DESC;", conn))
            {
                cmd.Parameters.Add("@Top", SqlDbType.Int).Value = top ?? int.MaxValue;
                cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        alerts.Add(ReadAlert(reader));
                }
            }
            return alerts;
        }

        /// <summary>Runs a learner action against the learner's own alerts. alertId null means all of them.</summary>
        public static int LearnerAction(int learnerId, int? alertId, string action)
        {
            string sql;
            switch (action)
            {
                case "Read":
                    sql = "UPDATE dbo.Alerts SET IsRead = 1 WHERE LearnerID = @LearnerID AND (@AlertID IS NULL OR AlertID = @AlertID);";
                    break;
                case "Acknowledge":
                    sql = "UPDATE dbo.Alerts SET IsRead = 1, IsAcknowledged = 1 WHERE LearnerID = @LearnerID AND (@AlertID IS NULL OR AlertID = @AlertID);";
                    break;
                case "Dismiss":
                    sql = "DELETE FROM dbo.Alerts WHERE LearnerID = @LearnerID AND (@AlertID IS NULL OR AlertID = @AlertID);";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }

            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@LearnerID", SqlDbType.Int).Value = learnerId;
                cmd.Parameters.Add("@AlertID", SqlDbType.Int).Value = alertId.HasValue ? (object)alertId.Value : DBNull.Value;
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // ========== Shared ==========

        private static void AddContentParameters(SqlCommand cmd, string title, string message, string alertType, int priority, DateTime? expiresAt)
        {
            cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = title;
            cmd.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = message;
            cmd.Parameters.Add("@AlertType", SqlDbType.NVarChar, 50).Value = alertType;
            cmd.Parameters.Add("@Priority", SqlDbType.TinyInt).Value = (byte)priority;
            cmd.Parameters.Add("@ExpiresAt", SqlDbType.DateTime2).Value = expiresAt.HasValue ? (object)expiresAt.Value : DBNull.Value;
        }

        private static AlertItem ReadAlert(SqlDataReader reader)
        {
            int priority = Convert.ToInt32(reader["Priority"]);
            string type = Convert.ToString(reader["AlertType"]);
            var createdAt = Convert.ToDateTime(reader["CreatedAt"]);
            DateTime? expiresAt = reader["ExpiresAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ExpiresAt"]);

            var item = new AlertItem
            {
                AlertID = Convert.ToInt32(reader["AlertID"]),
                Title = Convert.ToString(reader["Title"]),
                Message = Convert.ToString(reader["Message"]),
                AlertType = type,
                AlertTypeDisplay = type,
                TypeBadge = TypeBadge(type),
                Priority = priority,
                PriorityLevel = PriorityLevel(priority),
                PriorityClass = PriorityLevel(priority).ToLowerInvariant().Replace(" ", "-"),
                CreatedAt = createdAt,
                TimeAgo = UiHelper.TimeAgo(createdAt),
                ExpiresAt = expiresAt,
                IsActive = !expiresAt.HasValue || expiresAt.Value > DateTime.Now
            };

            if (HasColumn(reader, "IsRead"))
            {
                item.IsRead = Convert.ToBoolean(reader["IsRead"]);
                item.IsAcknowledged = Convert.ToBoolean(reader["IsAcknowledged"]);
            }
            return item;
        }

        private static bool HasColumn(SqlDataReader reader, string name)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static string PriorityLevel(int priority)
        {
            switch (priority)
            {
                case 1: return "Very Low";
                case 2: return "Low";
                case 3: return "Medium";
                case 4: return "High";
                case 5: return "Critical";
                default: return "Medium";
            }
        }

        public static string TypeBadge(string alertType)
        {
            switch (alertType)
            {
                case "Emergency": return "badge-danger";
                case "Reminder": return "badge-warning";
                case "Notification": return "badge-info";
                default: return "badge-secondary";
            }
        }
    }
}
