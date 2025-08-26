using ETicaret.Core.Entities;
using FluentValidation;

namespace ETicaret.Core.Validators
{
    public class UserValidator : AbstractValidator<AppUser>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad boş olamaz")
                .Length(2, 50).WithMessage("Ad 2-50 karakter arasında olmalıdır")
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Ad sadece harf içerebilir");

            RuleFor(x => x.SurName)
                .NotEmpty().WithMessage("Soyad boş olamaz")
                .Length(2, 50).WithMessage("Soyad 2-50 karakter arasında olmalıdır")
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Soyad sadece harf içerebilir");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi boş olamaz")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
                .MaximumLength(100).WithMessage("E-posta adresi çok uzun");

            RuleFor(x => x.Phone)
                .Matches(@"^(\+90|0)?[5][0-9]{9}$").WithMessage("Geçerli bir Türkiye telefon numarası giriniz");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz")
                .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Şifre en az bir küçük harf, bir büyük harf, bir rakam ve bir özel karakter içermelidir");

            RuleFor(x => x.UserName)
                .MaximumLength(50).WithMessage("Kullanıcı adı çok uzun")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir");
        }


    }
}
