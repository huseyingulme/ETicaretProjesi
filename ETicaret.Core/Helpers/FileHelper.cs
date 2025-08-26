using System.Security.Cryptography;
using System.Text;

namespace ETicaret.Core.Helpers
{
    public static class FileHelper
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
        private static readonly string[] AllowedDocumentExtensions = { ".pdf", ".doc", ".docx", ".txt", ".rtf" };
        private static readonly long MaxImageSize = 5 * 1024 * 1024; // 5MB
        private static readonly long MaxDocumentSize = 10 * 1024 * 1024; // 10MB

        public static bool IsValidImageFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return AllowedImageExtensions.Contains(extension);
        }

        public static bool IsValidDocumentFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return AllowedDocumentExtensions.Contains(extension);
        }

        public static bool IsValidFileSize(long fileSize, FileType fileType)
        {
            return fileType switch
            {
                FileType.Image => fileSize <= MaxImageSize,
                FileType.Document => fileSize <= MaxDocumentSize,
                _ => false
            };
        }

        public static string GenerateUniqueFileName(string originalFileName)
        {
            if (string.IsNullOrEmpty(originalFileName))
                throw new ArgumentException("Original file name cannot be null or empty", nameof(originalFileName));

            var extension = Path.GetExtension(originalFileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var randomString = GenerateRandomString(8);
            
            return $"{fileNameWithoutExtension}_{timestamp}_{randomString}{extension}";
        }

        public static string GetFileMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                ".rtf" => "application/rtf",
                _ => "application/octet-stream"
            };
        }

        public static string GetFileSizeString(long bytes)
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

        public static string CalculateFileHash(Stream fileStream)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(fileStream);
            return Convert.ToBase64String(hashBytes);
        }

        public static string CalculateFileHash(byte[] fileBytes)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(fileBytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool IsImageFile(Stream fileStream)
        {
            try
            {
                fileStream.Position = 0;
                var buffer = new byte[4];
                fileStream.Read(buffer, 0, 4);
                fileStream.Position = 0;

                // Check for common image file signatures
                return IsJpegSignature(buffer) || 
                       IsPngSignature(buffer) || 
                       IsGifSignature(buffer) || 
                       IsWebpSignature(buffer) ||
                       IsBmpSignature(buffer);
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> SaveFileAsync(Stream fileStream, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using var fileStreamWriter = new FileStream(filePath, FileMode.Create);
                await fileStream.CopyToAsync(fileStreamWriter);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<byte[]> ReadFileAsync(string filePath)
        {
            try
            {
                return await File.ReadAllBytesAsync(filePath);
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static DateTime GetFileCreationTime(string filePath)
        {
            try
            {
                return File.GetCreationTime(filePath);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime GetFileLastModifiedTime(string filePath)
        {
            try
            {
                return File.GetLastWriteTime(filePath);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static long GetFileSize(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                return fileInfo.Length;
            }
            catch
            {
                return 0;
            }
        }

        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new StringBuilder();

            foreach (var c in fileName)
            {
                if (!invalidChars.Contains(c))
                {
                    sanitized.Append(c);
                }
                else
                {
                    sanitized.Append('_');
                }
            }

            return sanitized.ToString();
        }

        public static string GetRelativePath(string fullPath, string basePath)
        {
            if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(basePath))
                return string.Empty;

            var fullPathUri = new Uri(fullPath);
            var basePathUri = new Uri(basePath);
            
            return basePathUri.MakeRelativeUri(fullPathUri).ToString();
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static bool IsJpegSignature(byte[] buffer)
        {
            return buffer.Length >= 2 && buffer[0] == 0xFF && buffer[1] == 0xD8;
        }

        private static bool IsPngSignature(byte[] buffer)
        {
            return buffer.Length >= 8 && 
                   buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                   buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A;
        }

        private static bool IsGifSignature(byte[] buffer)
        {
            return buffer.Length >= 6 && 
                   buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 &&
                   buffer[3] == 0x38 && (buffer[4] == 0x37 || buffer[4] == 0x39) && buffer[5] == 0x61;
        }

        private static bool IsWebpSignature(byte[] buffer)
        {
            return buffer.Length >= 12 &&
                   buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 &&
                   buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50;
        }

        private static bool IsBmpSignature(byte[] buffer)
        {
            return buffer.Length >= 2 && buffer[0] == 0x42 && buffer[1] == 0x4D;
        }
    }

    public enum FileType
    {
        Image,
        Document,
        Other
    }
}
