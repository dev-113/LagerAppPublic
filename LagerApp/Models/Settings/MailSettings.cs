#pragma warning disable CS8618
namespace LagerApp.Models.Settings
{
    public class MailSettings
    {
        public string FromAddress { get; set; }
        public string Password { get; set; }
        public string ToAddress { get; set; }
        public string SmtpClient { get; set; }
    }
}
