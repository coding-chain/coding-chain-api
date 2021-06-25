using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Dtos;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Read.Plagiarism;
using Application.Read.Plagiarism.Handlers;
using MediatR;

namespace Application.Write.Plagiarism
{
    public record PlagiarismGlobalAnalyseCommand() : IRequest<string?>;

    public class PlagiarismGlobalAnalyseHandler : IRequestHandler<PlagiarismGlobalAnalyseCommand, string?>
    {
        private readonly IDispatcher<PlagiarismAnalyzeExecutionDto> _plagiarismAnalysisDispatcherService;
        private readonly IReadSuspectFunctionRepository _readSuspectFunctionRepository;

        public PlagiarismGlobalAnalyseHandler(
            IDispatcher<PlagiarismAnalyzeExecutionDto> plagiarismAnalysisDispatcherService,
            IReadSuspectFunctionRepository readSuspectFunctionRepository)
        {
            _plagiarismAnalysisDispatcherService = plagiarismAnalysisDispatcherService;
            _readSuspectFunctionRepository = readSuspectFunctionRepository;
        }

        public async Task<string?> Handle(PlagiarismGlobalAnalyseCommand command,
            CancellationToken cancellationToken)
        {
            var newFunctions =
                await _readSuspectFunctionRepository.GetAllLastFunctionFiltered(new GetFunctionsQuery());
            foreach (var suspectFunction in newFunctions)
            {
                
                var olderFunctions = await _readSuspectFunctionRepository.GetAllLastFunctionFiltered(
                    new GetFunctionsQuery
                    {
                        ExcludedUserIdFilter = suspectFunction.LastEditorId,
                        LowerThanDateFilter = suspectFunction.LastModificationDate,
                        LanguageIdFilter = suspectFunction.LanguageId
                    });
                _plagiarismAnalysisDispatcherService.Dispatch(
                    new PlagiarismAnalyzeExecutionDto(
                        new Function(suspectFunction.Id, suspectFunction.Code),
                        olderFunctions.Select(f => new Function(f.Id, f.Code)).ToList()
                    ));
            }

            return null;
        }
    }
}