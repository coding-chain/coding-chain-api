using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Dtos;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Plagiarism.Handlers
{
    public record GetFunctionsToCompareRequest() : IRequest<IList<SuspectFunctionNavigation>>;

    public class GetFunctionsToCompare : IRequestHandler<GetFunctionsToCompareRequest, IList<SuspectFunctionNavigation>>
    {
        private readonly IReadSuspectFunctionRepository _readSuspectFunctionRepository;

        public GetFunctionsToCompare(IReadSuspectFunctionRepository readSuspectFunctionRepository)
        {
            _readSuspectFunctionRepository = readSuspectFunctionRepository;
        }

        public async Task<IList<SuspectFunctionNavigation>> Handle(GetFunctionsToCompareRequest request,
            CancellationToken cancellationToken)
        {
            return await _readSuspectFunctionRepository.GetAllLastFunctionFiltered(new GetFunctionsQuery());
        }
    }
}