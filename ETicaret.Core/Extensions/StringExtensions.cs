using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ETicaret.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToSlug(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Türkçe karakterleri değiştir
            text = text.Replace("ç", "c")
                      .Replace("ğ", "g")
                      .Replace("ı", "i")
                      .Replace("ö", "o")
                      .Replace("ş", "s")
                      .Replace("ü", "u")
                      .Replace("Ç", "C")
                      .Replace("Ğ", "G")
                      .Replace("İ", "I")
                      .Replace("Ö", "O")
                      .Replace("Ş", "S")
                      .Replace("Ü", "U");

            // Küçük harfe çevir
            text = text.ToLowerInvariant();

            // Özel karakterleri kaldır ve tire ile değiştir
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            text = text.Replace(" ", "-");

            // Çoklu tireleri tek tire yap
            text = Regex.Replace(text, @"-+", "-");

            return text.Trim('-');
        }

        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var cultureInfo = new CultureInfo("tr-TR");
            return cultureInfo.TextInfo.ToTitleCase(text.ToLower());
        }

        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - suffix.Length) + suffix;
        }

        public static string RemoveHtmlTags(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Regex.Replace(text, "<.*?>", string.Empty);
        }

        public static string CleanPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return string.Empty;

            // Sadece rakamları al
            var cleaned = Regex.Replace(phoneNumber, @"[^\d]", "");
            
            // Türkiye telefon numarası formatına çevir
            if (cleaned.StartsWith("0"))
            {
                cleaned = "+90" + cleaned.Substring(1);
            }
            else if (!cleaned.StartsWith("+90"))
            {
                cleaned = "+90" + cleaned;
            }

            return cleaned;
        }

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;

            var cleaned = phoneNumber.CleanPhoneNumber();
            return Regex.IsMatch(cleaned, @"^\+90[0-9]{10}$");
        }

        public static string GenerateRandomString(this int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ToBase64(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string base64Text)
        {
            if (string.IsNullOrEmpty(base64Text))
                return string.Empty;

            try
            {
                var bytes = Convert.FromBase64String(base64Text);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string MaskEmail(this string email)
        {
            if (string.IsNullOrEmpty(email) || !email.IsValidEmail())
                return email;

            var parts = email.Split('@');
            if (parts.Length != 2)
                return email;

            var username = parts[0];
            var domain = parts[1];

            if (username.Length <= 2)
                return email;

            var maskedUsername = username.Substring(0, 2) + new string('*', username.Length - 2);
            return $"{maskedUsername}@{domain}";
        }

        public static string MaskPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return phoneNumber;

            var cleaned = phoneNumber.CleanPhoneNumber();
            if (cleaned.Length < 10)
                return phoneNumber;

            return cleaned.Substring(0, 4) + new string('*', cleaned.Length - 7) + cleaned.Substring(cleaned.Length - 3);
        }

        public static string FormatCurrency(this decimal amount, string currency = "₺")
        {
            return $"{amount:N2} {currency}";
        }

        public static string FormatFileSize(this long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        public static bool ContainsIgnoreCase(this string source, string value)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
                return false;

            return source.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        public static string RemoveSpecialCharacters(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Regex.Replace(text, @"[^a-zA-Z0-9\s]", "");
        }

        public static string ToCamelCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
                return string.Empty;

            var result = words[0].ToLowerInvariant();
            for (int i = 1; i < words.Length; i++)
            {
                result += words[i].ToTitleCase().Replace(" ", "");
            }

            return result;
        }
    }
}
