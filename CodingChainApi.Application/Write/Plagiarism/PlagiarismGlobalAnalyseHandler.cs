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
        private readonly IReadFunctionRepository _readFunctionRepository;

        public PlagiarismGlobalAnalyseHandler(
            IDispatcher<PlagiarismAnalyzeExecutionDto> plagiarismAnalysisDispatcherService,
            IReadFunctionRepository readFunctionRepository)
        {
            _plagiarismAnalysisDispatcherService = plagiarismAnalysisDispatcherService;
            _readFunctionRepository = readFunctionRepository;
        }

        public async Task<string?> Handle(PlagiarismGlobalAnalyseCommand command,
            CancellationToken cancellationToken)
        {
            var newFunctions =
                await _readFunctionRepository.GetAllLastFunctionFiltered(new GetFunctionsQuery());
            foreach (var suspectFunction in newFunctions)
            {
                
                var olderFunctions = await _readFunctionRepository.GetAllLastFunctionFiltered(
                    new GetFunctionsQuery
                    {
                        ExcludedUserId = suspectFunction.LastEditorId,
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