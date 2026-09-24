using System;
using System.Security.Cryptography;
using System.Text;

namespace RespondX.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 32;
        private const int HashIterations = 10000;
        private const int HashSize = 32;

        /// <summary>
        /// Hashes a password with a generated salt
        /// </summary>
        public static string HashPassword(string password, out string salt)
        {
            // Generate salt
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

            // Hash password with salt
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes2 = Convert.FromBase64String(salt);
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes2.Length];
            Array.Copy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Array.Copy(saltBytes2, 0, combinedBytes, passwordBytes.Length, saltBytes2.Length);

            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);

                // Use PBKDF2 for stronger hashing
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes2, HashIterations, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(HashSize);
                    return Convert.ToBase64String(hash);
                }
            }
        }

        /// <summary>
        /// Verifies a password against a stored hash and salt
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            try
            {
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                byte[] storedHashBytes = Convert.FromBase64String(storedHash);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, HashIterations, HashAlgorithmName.SHA256))
                {
                    byte[] computedHash = pbkdf2.GetBytes(HashSize);

                    // Compare hashes in constant time
                    return ConstantTimeCompare(computedHash, storedHashBytes);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Constant time comparison to prevent timing attacks
        /// </summary>
        private static bool ConstantTimeCompare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        /// <summary>
        /// Generates a random password with specified length
        /// </summary>
        public static string GenerateRandomPassword(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var password = new char[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[length];
                rng.GetBytes(bytes);

                for (int i = 0; i < length; i++)
                {
                    password[i] = chars[bytes[i] % chars.Length];
                }
            }

            return new string(password);
        }

        /// <summary>
        /// Validates password strength
        /// </summary>
        public static bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        /// <summary>
        /// Gets password strength score (0-4)
        /// </summary>
        public static int GetPasswordStrengthScore(string password)
        {
            if (string.IsNullOrEmpty(password))
                return 0;

            int score = 0;

            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;

            bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            if (hasUpper) score++;
            if (hasLower) score++;
            if (hasDigit) score++;
            if (hasSpecial) score++;

            return Math.Min(score, 4);
        }

        /// <summary>
        /// Encrypts a string using AES
        /// </summary>
        public static string EncryptString(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(key))
                return plainText;

            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                byte[] iv = new byte[16];

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(iv);
                }

                using (var aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                        // Combine IV and ciphertext
                        byte[] combined = new byte[iv.Length + cipherBytes.Length];
                        Array.Copy(iv, 0, combined, 0, iv.Length);
                        Array.Copy(cipherBytes, 0, combined, iv.Length, cipherBytes.Length);

                        return Convert.ToBase64String(combined);
                    }
                }
            }
            catch
            {
                return plainText;
            }
        }

        /// <summary>
        /// Decrypts a string using AES
        /// </summary>
        public static string DecryptString(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(key))
                return cipherText;

            try
            {
                byte[] combined = Convert.FromBase64String(cipherText);
                byte[] iv = new byte[16];
                byte[] cipherBytes = new byte[combined.Length - 16];

                Array.Copy(combined, 0, iv, 0, 16);
                Array.Copy(combined, 16, cipherBytes, 0, cipherBytes.Length);

                byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));

                using (var aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(plainBytes);
                    }
                }
            }
            catch
            {
                return cipherText;
            }
        }
    }
}