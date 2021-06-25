namespace CodingChainApi.Infrastructure.Settings
{
    public interface IMailjetSettings
    {
        string SecretKey { get; set; }
        string ApiKey { get; set; }
        string SenderEmail { get; set; }
        string SenderName { get; set; }
    }

    public class MailjetSettings : IMailjetSettings
    {
        public string SecretKey { get; set; }
        public string ApiKey { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
    }
}