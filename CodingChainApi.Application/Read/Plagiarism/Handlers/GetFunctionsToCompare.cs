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
        private readonly IReadFunctionRepository _readFunctionRepository;

        public GetFunctionsToCompare(IReadFunctionRepository readFunctionRepository)
        {
            _readFunctionRepository = readFunctionRepository;
        }

        public async Task<IList<SuspectFunctionNavigation>> Handle(GetFunctionsToCompareRequest request,
            CancellationToken cancellationToken)
        {
            return await _readFunctionRepository.GetAllLastFunctionFiltered(new GetFunctionsQuery());
        }
    }
}