using System.ComponentModel.DataAnnotations;

namespace ETicaret.Core.Models
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            Attachments = new List<EmailAttachment>();
        }

        [Required]
        public List<string> To { get; set; }
        
        public List<string> Cc { get; set; }
        
        public List<string> Bcc { get; set; }
        
        [Required]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        public string Body { get; set; } = string.Empty;
        
        public bool IsHtml { get; set; } = true;
        
        public List<EmailAttachment> Attachments { get; set; }
        
        public string? FromName { get; set; }
        
        public string? FromEmail { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    }

    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
    }

    public enum EmailPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Urgent = 3
    }
}
