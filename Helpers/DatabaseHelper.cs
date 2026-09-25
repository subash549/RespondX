using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using RespondX.Models;

namespace RespondX.Helpers
{
    public static class DatabaseHelper
    {
        private static readonly Lazy<string> connectionString = new Lazy<string>(ResolveConnectionString);

        /// <summary>
        /// The single connection string used by every page and helper. Web.config must define
        /// "RespondX" (or "DefaultConnection"); there is deliberately no machine-specific fallback,
        /// so a missing setting fails loudly instead of silently pointing at a developer PC.
        /// </summary>
        public static string ConnectionString => connectionString.Value;

        private static string ResolveConnectionString()
        {
            var setting = ConfigurationManager.ConnectionStrings["RespondX"]
                ?? ConfigurationManager.ConnectionStrings["DefaultConnection"];

            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                throw new ConfigurationErrorsException("The RespondX database connection string is not configured in Web.config.");

            return setting.ConnectionString;
        }

        // ========== User Operations ==========

        public static User AuthenticateUser(string username, string password)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = @"
                        SELECT UserID, Username, Email, PasswordHash, Salt, FirstName, LastName, Role, IsActive, ProfileImage 
                        FROM Users 
                        WHERE Username = @Username OR Email = @Email";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", username);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new User
                                {
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    PasswordHash = reader["PasswordHash"].ToString(),
                                    Salt = reader["Salt"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Role = reader["Role"].ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    ProfileImage = reader["ProfileImage"] == DBNull.Value ? null : Convert.ToString(reader["ProfileImage"])
                                };

                                if (PasswordHelper.VerifyPassword(password, user.PasswordHash, user.Salt))
                                {
                                    return user;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("AuthenticateUser", ex.Message);
            }
            return null;
        }

        public static int CreateUser(User user)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = @"
                        INSERT INTO Users (Username, Email, PasswordHash, Salt, FirstName, LastName, Role, IsActive, CreatedAt)
                        VALUES (@Username, @Email, @PasswordHash, @Salt, @FirstName, @LastName, @Role, @IsActive, @CreatedAt);
                        SELECT SCOPE_IDENTITY();";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                        cmd.Parameters.AddWithValue("@Salt", user.Salt);
                        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", user.LastName);
                        cmd.Parameters.AddWithValue("@Role", user.Role);
                        cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        conn.Open();
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("CreateUser", ex.Message);
                return 0;
            }
        }

        public static bool UsernameExists(string username)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
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

        public static bool EmailExists(string email)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
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

        public static void UpdateLastLogin(int userId)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = "UPDATE Users SET LastLogin = @LastLogin WHERE UserID = @UserID";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("UpdateLastLogin", ex.Message);
            }
        }

        public static User GetUserById(int userId)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = @"
                        SELECT UserID, Username, Email, FirstName, LastName, Role, IsActive, CreatedAt, LastLogin, ProfileImage 
                        FROM Users 
                        WHERE UserID = @UserID";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Role = reader["Role"].ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    LastLogin = reader["LastLogin"] != DBNull.Value ? Convert.ToDateTime(reader["LastLogin"]) : (DateTime?)null,
                                    ProfileImage = reader["ProfileImage"] == DBNull.Value ? null : Convert.ToString(reader["ProfileImage"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("GetUserById", ex.Message);
            }
            return null;
        }

        public static void UpdateProfile(int userId, string firstName, string lastName, string email, string profileImage)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(@"
                UPDATE dbo.Users
                SET FirstName = @FirstName, LastName = @LastName, Email = @Email, ProfileImage = @ProfileImage
                WHERE UserID = @UserID;", conn))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = firstName;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = lastName;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                cmd.Parameters.Add("@ProfileImage", SqlDbType.NVarChar, 255).Value = (object)profileImage ?? DBNull.Value;
                conn.Open();
                if (cmd.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("The user account no longer exists.");
            }
        }

        public static void ChangePassword(int userId, string currentPassword, string newPassword)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                using (var cmd = new SqlCommand("SELECT PasswordHash, Salt FROM dbo.Users WITH (UPDLOCK, ROWLOCK) WHERE UserID = @UserID;", conn, transaction))
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read() || !PasswordHelper.VerifyPassword(currentPassword, Convert.ToString(reader["PasswordHash"]), Convert.ToString(reader["Salt"])))
                            throw new InvalidOperationException("Current password is incorrect.");
                    }

                    string salt;
                    string hash = PasswordHelper.HashPassword(newPassword, out salt);
                    using (var update = new SqlCommand("UPDATE dbo.Users SET PasswordHash = @Hash, Salt = @Salt WHERE UserID = @UserID;", conn, transaction))
                    {
                        update.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                        update.Parameters.Add("@Hash", SqlDbType.NVarChar, 255).Value = hash;
                        update.Parameters.Add("@Salt", SqlDbType.NVarChar, 50).Value = salt;
                        update.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        public static List<User> GetAllUsers()
        {
            var users = new List<User>();
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = @"
                        SELECT UserID, Username, Email, FirstName, LastName, Role, IsActive, CreatedAt, LastLogin, ProfileImage 
                        FROM Users 
                        ORDER BY CreatedAt DESC";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User
                                {
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Role = reader["Role"].ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    LastLogin = reader["LastLogin"] != DBNull.Value ? Convert.ToDateTime(reader["LastLogin"]) : (DateTime?)null,
                                    ProfileImage = reader["ProfileImage"] == DBNull.Value ? null : Convert.ToString(reader["ProfileImage"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("GetAllUsers", ex.Message);
            }
            return users;
        }

        // ========== Learner Operations ==========

        public static void CreateLearner(Models.Learner learner)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = @"
                        INSERT INTO Learners (LearnerID, DateOfBirth, PhoneNumber, Address, City, State, ZipCode, Organization, JobTitle, ExperienceYears, Certifications, Bio)
                        VALUES (@LearnerID, @DateOfBirth, @PhoneNumber, @Address, @City, @State, @ZipCode, @Organization, @JobTitle, @ExperienceYears, @Certifications, @Bio)";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LearnerID", learner.LearnerID);
                        cmd.Parameters.AddWithValue("@DateOfBirth", (object)learner.DateOfBirth ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PhoneNumber", (object)learner.PhoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)learner.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@City", (object)learner.City ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@State", (object)learner.State ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ZipCode", (object)learner.ZipCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Organization", (object)learner.Organization ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@JobTitle", (object)learner.JobTitle ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExperienceYears", learner.ExperienceYears);
                        cmd.Parameters.AddWithValue("@Certifications", (object)learner.Certifications ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Bio", (object)learner.Bio ?? DBNull.Value);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("CreateLearner", ex.Message);
            }
        }

        /// <summary>
        /// Creates the Learners profile row for a learner account if it is missing.
        /// Progress, quiz attempts and certificates reference Learners, so they fail without it.
        /// </summary>
        public static void EnsureLearnerProfile(int userId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM dbo.Learners WHERE LearnerID = @UserID)
                   AND EXISTS (SELECT 1 FROM dbo.Users WHERE UserID = @UserID)
                    INSERT INTO dbo.Learners (LearnerID, ExperienceYears) VALUES (@UserID, 0);", conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static Models.Learner GetLearnerById(int learnerId)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = @"
                        SELECT l.*, u.FirstName, u.LastName, u.Email, u.Username
                        FROM Learners l
                        INNER JOIN Users u ON l.LearnerID = u.UserID
                        WHERE l.LearnerID = @LearnerID";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LearnerID", learnerId);
                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Models.Learner
                                {
                                    LearnerID = Convert.ToInt32(reader["LearnerID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : (DateTime?)null,
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    City = reader["City"].ToString(),
                                    State = reader["State"].ToString(),
                                    ZipCode = reader["ZipCode"].ToString(),
                                    Organization = reader["Organization"].ToString(),
                                    JobTitle = reader["JobTitle"].ToString(),
                                    ExperienceYears = Convert.ToInt32(reader["ExperienceYears"]),
                                    Certifications = reader["Certifications"].ToString(),
                                    Bio = reader["Bio"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("GetLearnerById", ex.Message);
            }
            return null;
        }

        // ========== Module Operations ==========

        public static List<Models.Module> GetModules()
        {
            var modules = new List<Models.Module>();
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var query = @"
                        SELECT ModuleID, Title, Description, ModuleOrder, IsActive, CreatedAt, EstimatedHours, IsMandatory
                        FROM Modules
                        WHERE IsActive = 1
                        ORDER BY ModuleOrder";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                modules.Add(new Models.Module
                                {
                                    ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    ModuleOrder = Convert.ToInt32(reader["ModuleOrder"]),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    EstimatedHours = Convert.ToInt32(reader["EstimatedHours"]),
                                    IsMandatory = Convert.ToBoolean(reader["IsMandatory"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("GetModules", ex.Message);
            }
            return modules;
        }

        // ========== Logging Operations ==========

        public static void LogError(string source, string message, string stackTrace = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {source} - {message}");
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    System.Diagnostics.Debug.WriteLine(stackTrace);
                }
                ErrorLog.Write(source, message, stackTrace);
            }
            catch { /* Silently fail */ }
        }

        public static void LogInfo(string message, string source = "System")
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"INFO: {source} - {message}");
            }
            catch { /* Silently fail */ }
        }

        public static void LogFailedLoginAttempt(string username)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Failed login attempt: {username} at {DateTime.Now}");
            }
            catch { /* Silently fail */ }
        }
    }
}
