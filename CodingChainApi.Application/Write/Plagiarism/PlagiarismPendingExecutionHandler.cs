using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Dtos;
using Application.Contracts.IService;
using Application.Read.Plagiarism;
using MediatR;

namespace Application.Write.Plagiarism
{
    public record CodePlagiarismPendingExecutionRequest
        (Function Function, IList<Function> ComparedFunctions) : IRequest<Guid>;

    public class PlagiarismPendingExecutionHandler : IRequestHandler<CodePlagiarismPendingExecutionRequest, Guid>
    {
        private readonly IDispatcher<PlagiarismAnalyzeExecutionDto> _plagiarismAnalysisDispatcherService;

        public PlagiarismPendingExecutionHandler(
            IDispatcher<PlagiarismAnalyzeExecutionDto> plagiarismAnalysisDispatcherService)
        {
            _plagiarismAnalysisDispatcherService = plagiarismAnalysisDispatcherService;
        }

        public Task<Guid> Handle(CodePlagiarismPendingExecutionRequest request,
            CancellationToken cancellationToken)
        {
            _plagiarismAnalysisDispatcherService.Dispatch(
                new PlagiarismAnalyzeExecutionDto(request.Function, request.ComparedFunctions));
            return Task.FromResult(request.Function.Id);
        }
    }
}