using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Plagiarism.Handlers
{
    public record GetSuspectFunctionsPaginatedQuery() : GetFunctionsQuery,
        IRequest<IPagedList<SuspectFunctionNavigation>>;
    public class GetSuspectFunctionsPaginatedHandler: IRequestHandler<GetSuspectFunctionsPaginatedQuery, IPagedList<SuspectFunctionNavigation>>
    {
        private readonly IReadSuspectFunctionRepository _readSuspectFunctionRepository;

        public GetSuspectFunctionsPaginatedHandler(IReadSuspectFunctionRepository readSuspectFunctionRepository)
        {
            _readSuspectFunctionRepository = readSuspectFunctionRepository;
        }

        public Task<IPagedList<SuspectFunctionNavigation>> Handle(GetSuspectFunctionsPaginatedQuery request, CancellationToken cancellationToken)
        {
            return _readSuspectFunctionRepository.GetPaginatedLastSuspectFunctionsFiltered(request);
        }
        
        
    }
}