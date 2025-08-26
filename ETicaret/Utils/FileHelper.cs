using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace ETicaret.Utils
{
    public class FileHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        private string GetWebRootPath()
        {
            return _webHostEnvironment.WebRootPath;
        }

        public async Task<string> FileLoaderAsync(IFormFile fromFile, string filePath = "/Img")
        {
            string fileName = "";

            if (fromFile != null && fromFile.Length > 0)
            {
                try
                {

                    System.Diagnostics.Debug.WriteLine($"FileHelper: Dosya yükleniyor - {fromFile.FileName}, Boyut: {fromFile.Length}");
                    

                    string extension = Path.GetExtension(fromFile.FileName);
                    fileName = Guid.NewGuid().ToString() + extension;
                    

                    string webRootPath = GetWebRootPath();
                    string directory = Path.Combine(webRootPath, filePath.TrimStart('/'));
                    
                    System.Diagnostics.Debug.WriteLine($"FileHelper: WebRoot Path: {webRootPath}");
                    System.Diagnostics.Debug.WriteLine($"FileHelper: Hedef klasör: {directory}");
                    

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        System.Diagnostics.Debug.WriteLine($"FileHelper: Klasör oluşturuldu: {directory}");
                    }
                    
                    string fullPath = Path.Combine(directory, fileName);
                    System.Diagnostics.Debug.WriteLine($"FileHelper: Tam dosya yolu: {fullPath}");
                    
                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await fromFile.CopyToAsync(stream);
                    

                    if (File.Exists(fullPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"FileHelper: Dosya başarıyla oluşturuldu: {fullPath}");
                        return fileName;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"FileHelper: Dosya oluşturulamadı: {fullPath}");
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"FileHelper: Hata oluştu: {ex.Message}");
                    return "";
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("FileHelper: Dosya null veya boş");
            }
            
            return fileName;
        }

        public async Task<string> UploadLogoAsync(IFormFile logoFile, string brandName = "")
        {
            if (logoFile == null || logoFile.Length == 0)
                return "";

            try
            {

                string logoPath = "/Img/Brands";
                

                string extension = Path.GetExtension(logoFile.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = "";
                
                if (!string.IsNullOrEmpty(brandName))
                {

                    string safeBrandName = string.Join("", brandName.Split(Path.GetInvalidFileNameChars()));
                    fileName = $"{safeBrandName}_{timestamp}{extension}";
                }
                else
                {
                    fileName = $"logo_{timestamp}{extension}";
                }
                

                string webRootPath = GetWebRootPath();
                string directory = Path.Combine(webRootPath, logoPath.TrimStart('/'));
                

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                string fullPath = Path.Combine(directory, fileName);
                

                using var stream = new FileStream(fullPath, FileMode.Create);
                await logoFile.CopyToAsync(stream);
                

                if (File.Exists(fullPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Logo başarıyla yüklendi: {fullPath}");
                    return fileName;
                }
                
                return "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logo yüklenirken hata: {ex.Message}");
                return "";
            }
        }

        public bool FileRemover(string fileName, string filePath = "/Img/")
        {
            if (string.IsNullOrEmpty(fileName))
                return false;
                
            try
            {
                string webRootPath = GetWebRootPath();
                string directory = Path.Combine(webRootPath, filePath.TrimStart('/'));
                string fullPath = Path.Combine(directory, fileName);
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    System.Diagnostics.Debug.WriteLine($"Dosya silindi: {fullPath}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dosya silinirken hata: {ex.Message}");
                return false;
            }
        }

        public bool RemoveLogo(string logoFileName)
        {
            if (string.IsNullOrEmpty(logoFileName))
                return false;
                
            return FileRemover(logoFileName, "/Img/Brands/");
        }

        public async Task<string> UploadCategoryImageAsync(IFormFile imageFile, string categoryName = "")
        {
            if (imageFile == null || imageFile.Length == 0)
                return "";

            try
            {
                // Kategori resmi için özel klasör
                string imagePath = "/Img/Categories";
                
                // Benzersiz dosya adı oluştur (kategori adı + timestamp + extension)
                string extension = Path.GetExtension(imageFile.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = "";
                
                if (!string.IsNullOrEmpty(categoryName))
                {
                    // Kategori adından güvenli dosya adı oluştur
                    string safeCategoryName = string.Join("", categoryName.Split(Path.GetInvalidFileNameChars()));
                    fileName = $"{safeCategoryName}_{timestamp}{extension}";
                }
                else
                {
                    fileName = $"category_{timestamp}{extension}";
                }
                

                string webRootPath = GetWebRootPath();
                string directory = Path.Combine(webRootPath, imagePath.TrimStart('/'));
                

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                string fullPath = Path.Combine(directory, fileName);
                

                using var stream = new FileStream(fullPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                

                if (File.Exists(fullPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Kategori resmi başarıyla yüklendi: {fullPath}");
                    return fileName;
                }
                
                return "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Kategori resmi yüklenirken hata: {ex.Message}");
                return "";
            }
        }

        public bool RemoveCategoryImage(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
                return false;
                
            return FileRemover(imageFileName, "/Img/Categories/");
        }

        public async Task<string> UploadProductImageAsync(IFormFile imageFile, string productName = "")
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                System.Diagnostics.Debug.WriteLine("UploadProductImageAsync: Dosya null veya boş");
                return "";
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: Başlangıç - Dosya: {imageFile.FileName}, Boyut: {imageFile.Length}");
                
                // Ürün resmi için özel klasör
                string imagePath = "/Img/Products";
                
                // Benzersiz dosya adı oluştur (ürün adı + timestamp + extension)
                string extension = Path.GetExtension(imageFile.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = "";
                
                if (!string.IsNullOrEmpty(productName))
                {
                    // Ürün adından güvenli dosya adı oluştur
                    string safeProductName = string.Join("", productName.Split(Path.GetInvalidFileNameChars()));
                    fileName = $"{safeProductName}_{timestamp}{extension}";
                }
                else
                {
                    fileName = $"product_{timestamp}{extension}";
                }
                
                System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: Dosya adı oluşturuldu: {fileName}");
                

                string webRootPath = GetWebRootPath();
                if (string.IsNullOrEmpty(webRootPath))
                {
                    System.Diagnostics.Debug.WriteLine("UploadProductImageAsync: WebRoot path bulunamadı");
                    return "";
                }
                
                string directory = Path.Combine(webRootPath, imagePath.TrimStart('/'));
                
                System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: WebRoot Path: {webRootPath}");
                System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: Hedef klasör: {directory}");
                

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: Klasör oluşturuldu: {directory}");
                }
                
                string fullPath = Path.Combine(directory, fileName);
                System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: Tam dosya yolu: {fullPath}");
                

                using var stream = new FileStream(fullPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                stream.Close(); // Stream'i kapat
                

                if (File.Exists(fullPath))
                {
                    var fileInfo = new FileInfo(fullPath);
                    System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: Ürün resmi başarıyla yüklendi: {fullPath}, Boyut: {fileInfo.Length}");
                    
                    // Dosya boyutunu kontrol et
                    if (fileInfo.Length > 0)
                    {
                        return fileName;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("UploadProductImageAsync: Dosya boş");
                        // Boş dosyayı sil
                        try { File.Delete(fullPath); } catch { }
                        return "";
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: Dosya oluşturulamadı: {fullPath}");
                    return "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: Hata oluştu: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"UploadProductImageAsync: Stack trace: {ex.StackTrace}");
                return "";
            }
        }

        public bool RemoveProductImage(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
                return false;
                
            return FileRemover(imageFileName, "/Img/Products/");
        }

        public async Task<string> UploadSliderImageAsync(IFormFile imageFile, string sliderTitle = "")
        {
            if (imageFile == null || imageFile.Length == 0)
                return "";

            try
            {
                // Slider resmi için özel klasör
                string imagePath = "/Img/Sliders";
                
                // Benzersiz dosya adı oluştur (slider başlığı + timestamp + extension)
                string extension = Path.GetExtension(imageFile.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = "";
                
                if (!string.IsNullOrEmpty(sliderTitle))
                {
                    // Slider başlığından güvenli dosya adı oluştur
                    string safeSliderTitle = string.Join("", sliderTitle.Split(Path.GetInvalidFileNameChars()));
                    fileName = $"{safeSliderTitle}_{timestamp}{extension}";
                }
                else
                {
                    fileName = $"slider_{timestamp}{extension}";
                }
                

                string webRootPath = GetWebRootPath();
                string directory = Path.Combine(webRootPath, imagePath.TrimStart('/'));
                

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                string fullPath = Path.Combine(directory, fileName);
                

                using var stream = new FileStream(fullPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                

                if (File.Exists(fullPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Slider resmi başarıyla yüklendi: {fullPath}");
                    return fileName;
                }
                
                return "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Slider resmi yüklenirken hata: {ex.Message}");
                return "";
            }
        }

        public bool RemoveSliderImage(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
                return false;
                
            return FileRemover(imageFileName, "/Img/Sliders/");
        }



        public async Task<string> UploadContactImageAsync(IFormFile imageFile, string contactName = "")
        {
            if (imageFile == null || imageFile.Length == 0)
                return "";

            try
            {
                // Contact resmi için özel klasör
                string imagePath = "/Img/Contacts";
                
                // Benzersiz dosya adı oluştur (contact adı + timestamp + extension)
                string extension = Path.GetExtension(imageFile.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = "";
                
                if (!string.IsNullOrEmpty(contactName))
                {
                    // Contact adından güvenli dosya adı oluştur
                    string safeContactName = string.Join("", contactName.Split(Path.GetInvalidFileNameChars()));
                    fileName = $"{safeContactName}_{timestamp}{extension}";
                }
                else
                {
                    fileName = $"contact_{timestamp}{extension}";
                }
                

                string webRootPath = GetWebRootPath();
                string directory = Path.Combine(webRootPath, imagePath.TrimStart('/'));
                

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                string fullPath = Path.Combine(directory, fileName);
                

                using var stream = new FileStream(fullPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                

                if (File.Exists(fullPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Contact resmi başarıyla yüklendi: {fullPath}");
                    return fileName;
                }
                
                return "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Contact resmi yüklenirken hata: {ex.Message}");
                return "";
            }
        }

        public bool RemoveContactImage(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
                return false;
                
            return FileRemover(imageFileName, "/Img/Contacts/");
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Dosya boyutu kontrolü (5MB)
            if (file.Length > 5 * 1024 * 1024)
                return false;

            // Dosya türü kontrolü
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }

        public string GetFileSizeString(long bytes)
        {
            if (bytes == 0) return "0 Bytes";
            const int k = 1024;
            string[] sizes = { "Bytes", "KB", "MB", "GB" };
            int i = (int)Math.Floor(Math.Log(bytes) / Math.Log(k));
            return Math.Round(bytes / Math.Pow(k, i), 2) + " " + sizes[i];
        }

        // Tüm gerekli klasörleri oluştur
        public void EnsureDirectoriesExist()
        {
            try
            {
                string webRootPath = GetWebRootPath();
                var directories = new[]
                {
                    Path.Combine(webRootPath, "Img"),
                    Path.Combine(webRootPath, "Img", "Categories"),
                    Path.Combine(webRootPath, "Img", "Brands"),
                    Path.Combine(webRootPath, "Img", "Products"),
                    Path.Combine(webRootPath, "Img", "Sliders"),
                    Path.Combine(webRootPath, "Img", "Contacts")
                };

                foreach (var directory in directories)
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        System.Diagnostics.Debug.WriteLine($"Klasör oluşturuldu: {directory}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Klasörler oluşturulurken hata: {ex.Message}");
            }
        }
    }
}
