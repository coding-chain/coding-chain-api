using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.IService;
using CodingChainApi.Infrastructure.Settings;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Mailjet.Client.TransactionalEmails.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CodingChainApi.Infrastructure.Emailing
{
    public abstract class MailjetService<TContent> : IMailService<TContent>
    {
        private MailjetClient _mailjetClient;
        private ILogger<MailjetService<TContent>> _logger;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public MailjetService(IMailjetSettings settings, ILogger<MailjetService<TContent>> logger)
        {
            _mailjetClient = new MailjetClient(settings.ApiKey, settings.SecretKey);
            _senderEmail = settings.SenderEmail;
            _senderName = settings.SenderName;
            _logger = logger;
        }

        protected virtual string GetTextPart(TContent content) => "";
        protected virtual string GetHtmlPart(TContent content) => "";
        protected virtual string GetCustomId(TContent content) => "";

        public async Task SendMessage(Message<TContent> message)
        {
            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(_senderEmail, _senderName))
                .WithSubject(message.Subject)
                .WithTextPart(GetTextPart(message.Content))
                .WithHtmlPart(GetHtmlPart(message.Content))
                .WithCustomId(GetCustomId(message.Content))
                .WithTo(new SendContact(message.Contact.Email, message.Contact.Name))
                .Build();
            var response = await _mailjetClient.SendTransactionalEmailAsync(email);
            foreach (var responseMessage in response.Messages)
            {
                if (responseMessage.Status == "success")
                {
                    _logger.LogInformation("Message sent for: {Email}",
                        GetEmailList(responseMessage));
                }
                else
                {
                    _logger.LogInformation("Message fail for : {Email}",
                        GetEmailList(responseMessage));
                }
            }
        }

        private static string GetEmailList(MessageResult responseMessage)
        {
            return string.Join(";", responseMessage.To.Select(r => r.Email).ToList());
        }
    }
}