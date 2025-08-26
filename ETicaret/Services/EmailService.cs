using ETicaret.Core.Models;
using ETicaret.Core.Services;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ETicaret.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailMessage emailMessage)
        {
            try
            {
                using var client = CreateSmtpClient();
                using var mailMessage = CreateMailMessage(emailMessage);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", emailMessage.To));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", emailMessage.To));
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var emailMessage = new EmailMessage
            {
                To = new List<string> { toEmail },
                Subject = "E-Ticaret Sitemize Hoş Geldiniz!",
                Body = CreateWelcomeEmailBody(userName),
                IsHtml = true
            };

            return await SendEmailAsync(emailMessage);
        }

        public async Task<bool> SendOrderConfirmationEmailAsync(string toEmail, string userName, string orderNumber)
        {
            var emailMessage = new EmailMessage
            {
                To = new List<string> { toEmail },
                Subject = $"Sipariş Onayı - #{orderNumber}",
                Body = CreateOrderConfirmationEmailBody(userName, orderNumber),
                IsHtml = true
            };

            return await SendEmailAsync(emailMessage);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            var resetUrl = $"{_configuration["AppSettings:BaseUrl"]}/Account/ResetPassword?token={resetToken}";
            
            var emailMessage = new EmailMessage
            {
                To = new List<string> { toEmail },
                Subject = "Şifre Sıfırlama Talebi",
                Body = CreatePasswordResetEmailBody(resetUrl),
                IsHtml = true
            };

            return await SendEmailAsync(emailMessage);
        }

        public async Task<bool> SendContactFormEmailAsync(ContactViewModel contact)
        {
            var emailMessage = new EmailMessage
            {
                To = new List<string> { _configuration["AppSettings:AdminEmail"] ?? "admin@eticaret.com" },
                Subject = $"İletişim Formu - {contact.Subject}",
                Body = CreateContactFormEmailBody(contact),
                IsHtml = true
            };

            return await SendEmailAsync(emailMessage);
        }

        public async Task<bool> SendNewsletterEmailAsync(string toEmail, string subject, string content)
        {
            var emailMessage = new EmailMessage
            {
                To = new List<string> { toEmail },
                Subject = subject,
                Body = CreateNewsletterEmailBody(content),
                IsHtml = true
            };

            return await SendEmailAsync(emailMessage);
        }

        public async Task<bool> SendBulkEmailAsync(List<string> toEmails, string subject, string content)
        {
            var emailMessage = new EmailMessage
            {
                To = toEmails,
                Subject = subject,
                Body = CreateNewsletterEmailBody(content),
                IsHtml = true
            };

            return await SendEmailAsync(emailMessage);
        }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return await Task.FromResult(addr.Address == email);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsEmailDeliverableAsync(string email)
        {
            // Bu metod gerçek bir email doğrulama servisi ile entegre edilebilir
            // Şimdilik basit bir doğrulama yapıyoruz
            return await ValidateEmailAsync(email);
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";

            return new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };
        }

        private MailMessage CreateMailMessage(EmailMessage emailMessage)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@eticaret.com";
            var fromName = _configuration["EmailSettings:FromName"] ?? "E-Ticaret";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = emailMessage.Subject,
                Body = emailMessage.Body,
                IsBodyHtml = emailMessage.IsHtml,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            foreach (var to in emailMessage.To)
            {
                mailMessage.To.Add(to);
            }

            foreach (var cc in emailMessage.Cc)
            {
                mailMessage.CC.Add(cc);
            }

            foreach (var bcc in emailMessage.Bcc)
            {
                mailMessage.Bcc.Add(bcc);
            }

            return mailMessage;
        }

        private string CreateWelcomeEmailBody(string userName)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Hoş Geldiniz {userName}!</h2>
                    <p>E-Ticaret sitemize kayıt olduğunuz için teşekkür ederiz.</p>
                    <p>Artık tüm ürünlerimizi inceleyebilir, alışveriş yapabilir ve siparişlerinizi takip edebilirsiniz.</p>
                    <p>İyi alışverişler!</p>
                    <br>
                    <p>E-Ticaret Ekibi</p>
                </body>
                </html>";
        }

        private string CreateOrderConfirmationEmailBody(string userName, string orderNumber)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Siparişiniz Onaylandı!</h2>
                    <p>Sayın {userName},</p>
                    <p>Siparişiniz başarıyla alınmıştır.</p>
                    <p><strong>Sipariş Numarası:</strong> #{orderNumber}</p>
                    <p>Siparişinizin durumunu takip etmek için hesabınızdan giriş yapabilirsiniz.</p>
                    <p>Teşekkür ederiz!</p>
                    <br>
                    <p>E-Ticaret Ekibi</p>
                </body>
                </html>";
        }

        private string CreatePasswordResetEmailBody(string resetUrl)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Şifre Sıfırlama</h2>
                    <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
                    <p><a href='{resetUrl}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Şifremi Sıfırla</a></p>
                    <p>Bu bağlantı 24 saat geçerlidir.</p>
                    <p>Eğer bu talebi siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>
                    <br>
                    <p>E-Ticaret Ekibi</p>
                </body>
                </html>";
        }

        private string CreateContactFormEmailBody(ContactViewModel contact)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>İletişim Formu Mesajı</h2>
                    <p><strong>Ad Soyad:</strong> {contact.Name}</p>
                    <p><strong>E-posta:</strong> {contact.Email}</p>
                    <p><strong>Telefon:</strong> -</p>
                    <p><strong>Konu:</strong> {contact.Subject}</p>
                    <p><strong>Mesaj:</strong></p>
                    <p>{contact.Message}</p>
                    <p><strong>Gönderim Tarihi:</strong> {DateTime.Now:dd.MM.yyyy HH:mm}</p>
                </body>
                </html>";
        }

        private string CreateNewsletterEmailBody(string content)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>E-Ticaret Bülteni</h2>
                    <div>{content}</div>
                    <br>
                    <p>E-Ticaret Ekibi</p>
                </body>
                </html>";
        }
    }
}
