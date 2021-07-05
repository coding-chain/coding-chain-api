using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Write.Contracts;
using Domain.CodeAnalysis;
using Domain.Participations;
using MediatR;

namespace Application.Write.Plagiarism
{
    public record PlagiarizedFunction(Guid PlagiarizedFunctionId, bool? IsValid);

    public record UpdateSuspectFunctionValidityCommand(Guid SuspectFunctionId,
        IList<PlagiarizedFunction> PlagiarizedFunctionsStatuses) : IRequest<string>;

    public class UpdateSuspectFunctionValidityHandler : IRequestHandler<UpdateSuspectFunctionValidityCommand, string>
    {
        private readonly ISuspectFunctionRepository _suspectFunctionRepository;

        public UpdateSuspectFunctionValidityHandler(ISuspectFunctionRepository suspectFunctionRepository)
        {
            _suspectFunctionRepository = suspectFunctionRepository;
        }

        public async Task<string> Handle(UpdateSuspectFunctionValidityCommand request,
            CancellationToken cancellationToken)
        {
            var suspectFunction =
                await _suspectFunctionRepository.FindByIdAsync(new FunctionId(request.SuspectFunctionId));
            if (suspectFunction is null)
                throw new NotFoundException(request.SuspectFunctionId.ToString(), "SuspectFunction");
            var statuses = request.PlagiarizedFunctionsStatuses
                .Select(f => new PlagiarizedFunctionValidityStatus(new FunctionId(f.PlagiarizedFunctionId), f.IsValid));
            suspectFunction.SetPlagiarizedFunctionsValidities(statuses.ToList());
            await _suspectFunctionRepository.SetAsync(suspectFunction);
            return suspectFunction.Id.ToString();
        }
    }
}