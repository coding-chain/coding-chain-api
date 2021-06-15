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
        private readonly IReadFunctionRepository _readFunctionRepository;

        public GetSuspectFunctionsPaginatedHandler(IReadFunctionRepository readFunctionRepository)
        {
            _readFunctionRepository = readFunctionRepository;
        }

        public Task<IPagedList<SuspectFunctionNavigation>> Handle(GetSuspectFunctionsPaginatedQuery request, CancellationToken cancellationToken)
        {
            return _readFunctionRepository.GetPaginatedLastSuspectFunctionsFiltered(request);
        }
        
        
    }
}