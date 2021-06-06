using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Write.Plagiarism
{
    public record FunctionSimilarity(Guid Id, double Rate);

    public record PlagiarismDoneAnalyzeNotification
        (Guid SuspectedFunctionId, IList<FunctionSimilarity> ComparedFunctions) : INotification;

    public class PlagiarismExecutionDoneHandler : INotificationHandler<PlagiarismDoneAnalyzeNotification>
    {
        public Task Handle(PlagiarismDoneAnalyzeNotification notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}