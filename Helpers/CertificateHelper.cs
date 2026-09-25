using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using RespondX.Models;
using RespondX.Certificates;

namespace RespondX.Helpers
{
    public static class CertificateHelper
    {
        private static readonly string CertificatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "Generated");
        private static readonly string TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "Templates");

        static CertificateHelper()
        {
            // Ensure directories exist
            if (!Directory.Exists(CertificatePath))
                Directory.CreateDirectory(CertificatePath);

            if (!Directory.Exists(TemplatePath))
                Directory.CreateDirectory(TemplatePath);
        }

        /// <summary>
        /// Generates a certificate for a learner
        /// </summary>
        public static string GenerateCertificate(Certificate certificate)
        {
            try
            {
                // Generate unique certificate number
                certificate.CertificateNumber = GenerateCertificateNumber();
                certificate.VerificationCode = GenerateVerificationCode();
                certificate.IssueDate = DateTime.Now;
                certificate.ExpiryDate = DateTime.Now.AddYears(2);

                // Generate certificate image
                string imagePath = CertificateTemplate.SaveCertificateImage(certificate);

                // Generate HTML version
                string htmlPath = CertificateTemplate.SaveCertificateHtml(certificate);

                if (!string.IsNullOrEmpty(imagePath))
                {
                    certificate.PdfPath = imagePath;
                }

                return imagePath;
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Certificate Generation Error", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Generates a unique certificate number
        /// </summary>
        private static string GenerateCertificateNumber()
        {
            var random = new Random();
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var randomPart = random.Next(10000, 99999).ToString();
            return $"RX-{timestamp}-{randomPart}";
        }

        /// <summary>
        /// Generates a verification code
        /// </summary>
        private static string GenerateVerificationCode()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var code = new StringBuilder(12);

            for (int i = 0; i < 12; i++)
            {
                code.Append(chars[random.Next(chars.Length)]);
            }

            return code.ToString();
        }

        /// <summary>
        /// Verifies a certificate by code
        /// </summary>
        public static bool VerifyCertificate(string verificationCode)
        {
            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    var query = "SELECT COUNT(*) FROM Certificates WHERE VerificationCode = @Code AND IsActive = 1";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Code", verificationCode);
                        conn.Open();
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets certificate by verification code
        /// </summary>
        public static Certificate GetCertificateByCode(string verificationCode)
        {
            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    var query = @"
                        SELECT c.*, u.FirstName + ' ' + u.LastName as LearnerName, m.Title as ModuleTitle
                        FROM Certificates c
                        INNER JOIN Learners l ON c.LearnerID = l.LearnerID
                        INNER JOIN Users u ON l.LearnerID = u.UserID
                        INNER JOIN Modules m ON c.ModuleID = m.ModuleID
                        WHERE c.VerificationCode = @Code AND c.IsActive = 1";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Code", verificationCode);
                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Certificate
                                {
                                    CertificateID = Convert.ToInt32(reader["CertificateID"]),
                                    LearnerID = Convert.ToInt32(reader["LearnerID"]),
                                    ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                    CertificateNumber = reader["CertificateNumber"].ToString(),
                                    VerificationCode = reader["VerificationCode"].ToString(),
                                    IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                                    ExpiryDate = reader["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpiryDate"]) : (DateTime?)null,
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    Score = Convert.ToDecimal(reader["Score"]),
                                    LearnerName = reader["LearnerName"].ToString(),
                                    ModuleTitle = reader["ModuleTitle"].ToString(),
                                    PdfPath = reader["PdfPath"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public static Certificate GetEligibleCertificate(int certificateId, int learnerId, decimal minimumScoreExclusive)
        {
            const string query = @"
                SELECT c.*, u.FirstName + N' ' + u.LastName AS LearnerName, m.Title AS ModuleTitle
                FROM Certificates c
                INNER JOIN Learners l ON c.LearnerID = l.LearnerID
                INNER JOIN Users u ON l.LearnerID = u.UserID
                INNER JOIN Modules m ON c.ModuleID = m.ModuleID
                WHERE c.CertificateID = @CertificateID
                  AND c.LearnerID = @LearnerID
                  AND c.IsActive = 1
                  AND c.Score > @MinimumScore;";

            return GetSingleCertificate(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@CertificateID", certificateId);
                cmd.Parameters.AddWithValue("@LearnerID", learnerId);
                cmd.Parameters.AddWithValue("@MinimumScore", minimumScoreExclusive);
            });
        }

        public static Certificate GetEligibleCertificateForModule(int learnerId, int moduleId, decimal minimumScoreExclusive)
        {
            const string query = @"
                SELECT TOP 1 c.*, u.FirstName + N' ' + u.LastName AS LearnerName, m.Title AS ModuleTitle
                FROM Certificates c
                INNER JOIN Learners l ON c.LearnerID = l.LearnerID
                INNER JOIN Users u ON l.LearnerID = u.UserID
                INNER JOIN Modules m ON c.ModuleID = m.ModuleID
                WHERE c.LearnerID = @LearnerID
                  AND c.ModuleID = @ModuleID
                  AND c.IsActive = 1
                  AND c.Score > @MinimumScore
                ORDER BY c.IssueDate DESC, c.CertificateID DESC;";

            return GetSingleCertificate(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@LearnerID", learnerId);
                cmd.Parameters.AddWithValue("@ModuleID", moduleId);
                cmd.Parameters.AddWithValue("@MinimumScore", minimumScoreExclusive);
            });
        }

        private static Certificate GetSingleCertificate(string query, Action<SqlCommand> addParameters)
        {
            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    addParameters(cmd);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return new Certificate
                        {
                            CertificateID = Convert.ToInt32(reader["CertificateID"]),
                            LearnerID = Convert.ToInt32(reader["LearnerID"]),
                            ModuleID = Convert.ToInt32(reader["ModuleID"]),
                            CertificateNumber = reader["CertificateNumber"].ToString(),
                            VerificationCode = reader["VerificationCode"].ToString(),
                            IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                            ExpiryDate = reader["ExpiryDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ExpiryDate"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            Score = Convert.ToDecimal(reader["Score"]),
                            PdfPath = reader["PdfPath"] == DBNull.Value ? null : reader["PdfPath"].ToString(),
                            LearnerName = reader["LearnerName"].ToString(),
                            ModuleTitle = reader["ModuleTitle"].ToString(),
                            Issuer = "RespondX Training Institute"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Eligible certificate retrieval error", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets certificate image as base64 string
        /// </summary>
        public static string GetCertificateImage(string certificatePath)
        {
            try
            {
                if (!File.Exists(certificatePath))
                    return null;

                byte[] imageBytes = File.ReadAllBytes(certificatePath);
                return Convert.ToBase64String(imageBytes);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a certificate record in the database
        /// </summary>
        public static bool SaveCertificate(Certificate certificate)
        {
            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    var query = @"
                        INSERT INTO Certificates (LearnerID, ModuleID, CertificateNumber, IssueDate, ExpiryDate, 
                                                  IsActive, PdfPath, VerificationCode, Score)
                        VALUES (@LearnerID, @ModuleID, @CertificateNumber, @IssueDate, @ExpiryDate, 
                                @IsActive, @PdfPath, @VerificationCode, @Score);
                        SELECT SCOPE_IDENTITY();";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LearnerID", certificate.LearnerID);
                        cmd.Parameters.AddWithValue("@ModuleID", certificate.ModuleID);
                        cmd.Parameters.AddWithValue("@CertificateNumber", certificate.CertificateNumber);
                        cmd.Parameters.AddWithValue("@IssueDate", certificate.IssueDate);
                        cmd.Parameters.AddWithValue("@ExpiryDate", (object)certificate.ExpiryDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", certificate.IsActive);
                        cmd.Parameters.AddWithValue("@PdfPath", (object)certificate.PdfPath ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@VerificationCode", certificate.VerificationCode);
                        cmd.Parameters.AddWithValue("@Score", certificate.Score);

                        conn.Open();
                        var result = cmd.ExecuteScalar();
                        certificate.CertificateID = Convert.ToInt32(result);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Certificate Save Error", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets certificates for a learner
        /// </summary>
        public static List<Certificate> GetCertificatesByLearner(int learnerId)
        {
            var certificates = new List<Certificate>();

            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    var query = @"
                        SELECT c.*, u.FirstName + ' ' + u.LastName as LearnerName, m.Title as ModuleTitle
                        FROM Certificates c
                        INNER JOIN Learners l ON c.LearnerID = l.LearnerID
                        INNER JOIN Users u ON l.LearnerID = u.UserID
                        INNER JOIN Modules m ON c.ModuleID = m.ModuleID
                        WHERE c.LearnerID = @LearnerID AND c.IsActive = 1
                        ORDER BY c.IssueDate DESC";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LearnerID", learnerId);
                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                certificates.Add(new Certificate
                                {
                                    CertificateID = Convert.ToInt32(reader["CertificateID"]),
                                    LearnerID = Convert.ToInt32(reader["LearnerID"]),
                                    ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                    CertificateNumber = reader["CertificateNumber"].ToString(),
                                    IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                                    ExpiryDate = reader["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpiryDate"]) : (DateTime?)null,
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    PdfPath = reader["PdfPath"].ToString(),
                                    VerificationCode = reader["VerificationCode"].ToString(),
                                    Score = Convert.ToDecimal(reader["Score"]),
                                    LearnerName = reader["LearnerName"].ToString(),
                                    ModuleTitle = reader["ModuleTitle"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Certificate Retrieval Error", ex.Message);
            }

            return certificates;
        }

        /// <summary>
        /// Gets all certificates
        /// </summary>
        public static List<Certificate> GetAllCertificates()
        {
            var certificates = new List<Certificate>();

            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    var query = @"
                        SELECT c.*, u.FirstName + ' ' + u.LastName as LearnerName, m.Title as ModuleTitle
                        FROM Certificates c
                        INNER JOIN Learners l ON c.LearnerID = l.LearnerID
                        INNER JOIN Users u ON l.LearnerID = u.UserID
                        INNER JOIN Modules m ON c.ModuleID = m.ModuleID
                        ORDER BY c.IssueDate DESC";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                certificates.Add(new Certificate
                                {
                                    CertificateID = Convert.ToInt32(reader["CertificateID"]),
                                    LearnerID = Convert.ToInt32(reader["LearnerID"]),
                                    ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                    CertificateNumber = reader["CertificateNumber"].ToString(),
                                    IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                                    ExpiryDate = reader["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpiryDate"]) : (DateTime?)null,
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    PdfPath = reader["PdfPath"].ToString(),
                                    VerificationCode = reader["VerificationCode"].ToString(),
                                    Score = Convert.ToDecimal(reader["Score"]),
                                    LearnerName = reader["LearnerName"].ToString(),
                                    ModuleTitle = reader["ModuleTitle"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.LogError("Certificate Retrieval Error", ex.Message);
            }

            return certificates;
        }

        /// <summary>
        /// Revokes a certificate
        /// </summary>
        public static bool RevokeCertificate(int certificateId)
        {
            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    var query = "UPDATE Certificates SET IsActive = 0 WHERE CertificateID = @CertificateID";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CertificateID", certificateId);
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates certificate verification badge HTML
        /// </summary>
        public static string GenerateVerificationBadge(string verificationCode, bool isValid)
        {
            if (isValid)
            {
                return $@"
                <div class='certificate-verified'>
                    <i class='fas fa-check-circle' style='color:#48bb78;'></i>
                    <span style='font-weight:600;color:#22543d;'>Verified</span>
                    <span style='font-size:12px;color:#718096;margin-left:10px;'>Code: {verificationCode}</span>
                </div>";
            }
            else
            {
                return $@"
                <div class='certificate-invalid'>
                    <i class='fas fa-times-circle' style='color:#fc8181;'></i>
                    <span style='font-weight:600;color:#9b2c2c;'>Not Verified</span>
                    <span style='font-size:12px;color:#718096;margin-left:10px;'>Code: {verificationCode}</span>
                </div>";
            }
        }
    }
}
