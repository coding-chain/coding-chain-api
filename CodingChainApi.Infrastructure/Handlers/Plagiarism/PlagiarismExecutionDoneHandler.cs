using System.Threading;
using System.Threading.Tasks;
using Application.Write.Plagiarism;
using CodingChainApi.Infrastructure.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Handlers.Plagiarism
{
    public class PlagiarismExecutionDoneHandler : INotificationHandler<PlagiarismDoneAnalyzeNotification>
    {
        private readonly IHubContext<PlagiarismHub, IPlagiarismClient> _hub;


        public PlagiarismExecutionDoneHandler(IHubContext<PlagiarismHub, IPlagiarismClient> hub)
        {
            _hub = hub;
        }

        public async Task Handle(PlagiarismDoneAnalyzeNotification notification, CancellationToken cancellationToken)
        {
            await _hub.Clients.All.OnPlagiarismAnalyseReady(new PlagiarismAnalyzeReadyEvent());
        }
    }
}