using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Contracts.Dtos;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Read.Plagiarism.Handlers;
using MediatR;

namespace Application.Write.Plagiarism
{
    public record PlagiarismAnalyseCommand
        (Guid SuspectFunctionId, IList<Guid>? FunctionsIds = null) : IRequest<string?>;

    public class PlagiarismAnalyseHandler : IRequestHandler<PlagiarismAnalyseCommand, string?>
    {
        private readonly IDispatcher<PlagiarismAnalyzeExecutionDto> _dispatcher;
        private readonly IReadFunctionRepository _readFunctionRepository;

        public PlagiarismAnalyseHandler(
            IDispatcher<PlagiarismAnalyzeExecutionDto> dispatcher,
            IReadFunctionRepository readFunctionRepository)
        {
            _dispatcher = dispatcher;
            _readFunctionRepository = readFunctionRepository;
        }

        public async Task<string?> Handle(PlagiarismAnalyseCommand command,
            CancellationToken cancellationToken)
        {
            var suspectFunction = await _readFunctionRepository.GetLastByFunctionId(command.SuspectFunctionId);
            if (suspectFunction is null)
                throw new NotFoundException(command.SuspectFunctionId.ToString(), "SuspectFunction");

            var otherFunctions = await _readFunctionRepository.GetAllLastFunctionFiltered(new GetFunctionsQuery
            {
                ExcludedUserId = suspectFunction.LastEditorId,
                LowerThanDateFilter = suspectFunction.LastModificationDate,
                FunctionsIds = command.FunctionsIds,
                LanguageIdFilter = suspectFunction.LanguageId
            });
            
            _dispatcher.Dispatch(new PlagiarismAnalyzeExecutionDto(
                new Function(suspectFunction.Id, suspectFunction.Code),
                otherFunctions.Select(f => new Function(f.Id, f.Code)).ToList()
                ));
            return null;
        }
    }
}