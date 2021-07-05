using Application.Contracts.Dtos;
using CodingChainApi.Infrastructure.Settings;
using Mailjet.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CodingChainApi.Infrastructure.Emailing
{
    public class SuspectFunctionMailService : MailjetService<SuspectFunctionContent>
    {
        public SuspectFunctionMailService(IMailjetSettings settings, ILogger<MailjetService<SuspectFunctionContent>> logger) : base(settings, logger)
        {
        }
        protected override string GetTextPart(SuspectFunctionContent content)
        {
            return content.Function.Code;
        }

        
    }
}