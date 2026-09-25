using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace RespondX.Helpers
{
    public static class ModuleMediaHelper
    {
        private static string ConnectionString => DatabaseHelper.ConnectionString;

        public static void EnsureModuleMediaColumns()
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    AddColumnIfMissing(conn, "Modules", "YouTubeVideoUrl", "NVARCHAR(500) NULL");
                    AddColumnIfMissing(conn, "Modules", "VideoFileUrl", "NVARCHAR(500) NULL");
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("EnsureModuleMediaColumns", ex.Message);
            }
        }

        private static void AddColumnIfMissing(SqlConnection conn, string table, string column, string definition)
        {
            using (var check = new SqlCommand(
                "SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @Table AND COLUMN_NAME = @Column", conn))
            {
                check.Parameters.AddWithValue("@Table", table);
                check.Parameters.AddWithValue("@Column", column);
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    using (var alter = new SqlCommand($"ALTER TABLE [{table}] ADD [{column}] {definition}", conn))
                    {
                        alter.ExecuteNonQuery();
                    }
                }
            }
        }

        public static string GetYouTubeEmbedUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            url = url.Trim();
            var match = Regex.Match(url, @"(?:youtube\.com\/(?:watch\?(?:.*&)?v=|embed\/|shorts\/)|youtu\.be\/)([A-Za-z0-9_-]{11})",
                RegexOptions.IgnoreCase);
            if (!match.Success)
                return null;

            return "https://www.youtube.com/embed/" + match.Groups[1].Value;
        }

        public static string SaveUploadedVideo(HttpPostedFile file, int moduleId)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            var allowed = new[] { ".mp4", ".webm", ".ogg", ".mov" };
            if (string.IsNullOrEmpty(extension) || Array.IndexOf(allowed, extension) < 0)
                throw new InvalidOperationException("Only MP4, WebM, OGG, or MOV video files are allowed.");

            const int maxBytes = 100 * 1024 * 1024;
            if (file.ContentLength > maxBytes)
                throw new InvalidOperationException("Video file must be 100 MB or smaller.");

            var uploadRoot = HttpContext.Current.Server.MapPath("~/Uploads/Modules");
            if (!Directory.Exists(uploadRoot))
                Directory.CreateDirectory(uploadRoot);

            var fileName = $"module_{moduleId}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
            var physicalPath = Path.Combine(uploadRoot, fileName);
            file.SaveAs(physicalPath);

            return "~/Uploads/Modules/" + fileName;
        }

        public static string GetLessonSummary(string content, int maxLength = 120)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var plain = Regex.Replace(content, "<.*?>", " ");
            plain = Regex.Replace(plain, @"\s+", " ").Trim();
            if (plain.Length <= maxLength)
                return plain;

            return plain.Substring(0, maxLength - 3) + "...";
        }
    }
}
