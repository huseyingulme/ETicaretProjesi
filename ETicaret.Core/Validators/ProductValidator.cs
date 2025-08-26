using ETicaret.Core.Entities;
using FluentValidation;

namespace ETicaret.Core.Validators
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı boş olamaz")
                .Length(2, 200).WithMessage("Ürün adı 2-200 karakter arasında olmalıdır");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Ürün açıklaması boş olamaz")
                .Length(10, 2000).WithMessage("Ürün açıklaması 10-2000 karakter arasında olmalıdır");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Ürün fiyatı 0'dan büyük olmalıdır")
                .LessThan(1000000).WithMessage("Ürün fiyatı çok yüksek");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz")
                .LessThan(100000).WithMessage("Stok miktarı çok yüksek");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Kategori seçilmelidir");

            RuleFor(x => x.BrandId)
                .GreaterThan(0).WithMessage("Marka seçilmelidir");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Ürün resmi seçilmelidir")
                .Must(BeAValidImageUrl).WithMessage("Geçerli bir resim URL'si giriniz");

            RuleFor(x => x.ProductCode)
                .MaximumLength(50).WithMessage("Ürün kodu çok uzun")
                .Matches(@"^[A-Z0-9-_]*$").WithMessage("Ürün kodu sadece büyük harf, rakam, tire ve alt çizgi içerebilir");
        }

        private bool BeAValidImageUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return false;

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(imageUrl).ToLowerInvariant();
            
            return validExtensions.Contains(extension) || 
                   imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase);
        }
    }
}
