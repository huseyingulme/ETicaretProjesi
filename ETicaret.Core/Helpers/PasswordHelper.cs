using System.Security.Cryptography;
using System.Text;

namespace ETicaret.Core.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 32;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            var hashBytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(key, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                var hashBytes = Convert.FromBase64String(hashedPassword);

                if (hashBytes.Length != SaltSize + KeySize)
                    return false;

                var salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                var storedKey = new byte[KeySize];
                Array.Copy(hashBytes, SaltSize, storedKey, 0, KeySize);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                var key = pbkdf2.GetBytes(KeySize);

                return CryptographicOperations.FixedTimeEquals(key, storedKey);
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }

        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            var hasLower = password.Any(char.IsLower);
            var hasUpper = password.Any(char.IsUpper);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c));

            return hasLower && hasUpper && hasDigit && hasSpecial;
        }

        public static string GeneratePasswordResetToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var tokenBytes = new byte[32];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }

        public static string GenerateEmailVerificationToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var tokenBytes = new byte[16];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }

        public static string MaskPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            if (password.Length <= 2)
                return new string('*', password.Length);

            return password.Substring(0, 1) + new string('*', password.Length - 2) + password.Substring(password.Length - 1);
        }

        public static int GetPasswordStrengthScore(string password)
        {
            if (string.IsNullOrEmpty(password))
                return 0;

            int score = 0;

            // Uzunluk kontrolü
            if (password.Length >= 8) score += 1;
            if (password.Length >= 12) score += 1;
            if (password.Length >= 16) score += 1;

            // Karakter çeşitliliği
            if (password.Any(char.IsLower)) score += 1;
            if (password.Any(char.IsUpper)) score += 1;
            if (password.Any(char.IsDigit)) score += 1;
            if (password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c))) score += 1;

            // Tekrar eden karakter kontrolü
            if (!HasRepeatingCharacters(password)) score += 1;

            // Sıralı karakter kontrolü
            if (!HasSequentialCharacters(password)) score += 1;

            return Math.Min(score, 10); // Maksimum 10 puan
        }

        private static bool HasRepeatingCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (password[i] == password[i + 1] && password[i + 1] == password[i + 2])
                    return true;
            }
            return false;
        }

        private static bool HasSequentialCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (char.IsLetter(password[i]) && char.IsLetter(password[i + 1]) && char.IsLetter(password[i + 2]))
                {
                    if (password[i + 1] == password[i] + 1 && password[i + 2] == password[i] + 2)
                        return true;
                }
            }
            return false;
        }

        public static string GetPasswordStrengthText(int score)
        {
            return score switch
            {
                <= 2 => "Çok Zayıf",
                <= 4 => "Zayıf",
                <= 6 => "Orta",
                <= 8 => "Güçlü",
                _ => "Çok Güçlü"
            };
        }
    }
}
