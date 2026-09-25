using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RespondX.Models;

namespace RespondX.Helpers
{
    public static class NotificationRepository
    {
        public static void NotifyRoles(string notificationType, string title, string message, string targetUrl, int? actorUserId, params string[] roles)
        {
            if (roles == null || roles.Length == 0) return;
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                INSERT INTO dbo.Notifications (RecipientUserID, ActorUserID, NotificationType, Title, Message, TargetUrl)
                SELECT u.UserID, @ActorUserID, @Type, @Title, @Message, @TargetUrl
                FROM dbo.Users u
                WHERE u.IsActive = 1 AND u.Role IN (" + string.Join(",", System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, roles.Length), i => "@Role" + i)) + ");", conn))
            {
                cmd.Parameters.Add("@ActorUserID", SqlDbType.Int).Value = actorUserId.HasValue ? (object)actorUserId.Value : DBNull.Value;
                cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 30).Value = notificationType;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 150).Value = title;
                cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 500).Value = message;
                cmd.Parameters.Add("@TargetUrl", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(targetUrl) ? (object)DBNull.Value : targetUrl;
                for (int i = 0; i < roles.Length; i++)
                    cmd.Parameters.Add("@Role" + i, SqlDbType.NVarChar, 20).Value = roles[i];
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static int GetUnreadCount(int userId)
        {
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Notifications WHERE RecipientUserID = @UserID AND ReadAt IS NULL;", conn))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static List<NotificationItem> GetRecent(int userId, int top = 10)
        {
            var items = new List<NotificationItem>();
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT TOP (@Top) NotificationID, NotificationType, Title, Message, TargetUrl, CreatedAt, ReadAt
                FROM dbo.Notifications WHERE RecipientUserID = @UserID
                ORDER BY CreatedAt DESC, NotificationID DESC;", conn))
            {
                cmd.Parameters.Add("@Top", SqlDbType.Int).Value = top;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new NotificationItem
                        {
                            NotificationID = Convert.ToInt32(reader["NotificationID"]),
                            NotificationType = Convert.ToString(reader["NotificationType"]),
                            Title = Convert.ToString(reader["Title"]),
                            Message = Convert.ToString(reader["Message"]),
                            TargetUrl = reader["TargetUrl"] == DBNull.Value ? null : Convert.ToString(reader["TargetUrl"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            IsRead = reader["ReadAt"] != DBNull.Value
                        });
                    }
                }
            }
            return items;
        }

        public static void MarkRead(int userId, int notificationId)
        {
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand("UPDATE dbo.Notifications SET ReadAt = GETDATE() WHERE RecipientUserID = @UserID AND NotificationID = @ID AND ReadAt IS NULL;", conn))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = notificationId;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void MarkAllRead(int userId)
        {
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            using (var cmd = new SqlCommand("UPDATE dbo.Notifications SET ReadAt = GETDATE() WHERE RecipientUserID = @UserID AND ReadAt IS NULL;", conn))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
