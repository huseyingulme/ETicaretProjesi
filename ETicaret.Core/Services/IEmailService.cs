using ETicaret.Core.Models;

namespace ETicaret.Core.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailMessage emailMessage);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendOrderConfirmationEmailAsync(string toEmail, string userName, string orderNumber);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken);
        Task<bool> SendContactFormEmailAsync(ContactViewModel contact);
        Task<bool> SendNewsletterEmailAsync(string toEmail, string subject, string content);
        Task<bool> SendBulkEmailAsync(List<string> toEmails, string subject, string content);
        Task<bool> ValidateEmailAsync(string email);
        Task<bool> IsEmailDeliverableAsync(string email);
    }
}
