using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Plagiarism.Handlers
{
    public record GetLastPlagiarizedFunctionsByFunctionQuery(Guid FunctionId) : PaginationQueryBase,
        IRequest<IPagedList<PlagiarizedFunctionNavigation>>;

    public class GetPlagiarizedFunctionsByFunctionHandler : IRequestHandler<GetLastPlagiarizedFunctionsByFunctionQuery,
        IPagedList<PlagiarizedFunctionNavigation>>
    {
        private IReadSuspectFunctionRepository _readSuspectFunctionRepository;

        public GetPlagiarizedFunctionsByFunctionHandler(IReadSuspectFunctionRepository readSuspectFunctionRepository)
        {
            _readSuspectFunctionRepository = readSuspectFunctionRepository;
        }

        public Task<IPagedList<PlagiarizedFunctionNavigation>> Handle(GetLastPlagiarizedFunctionsByFunctionQuery request,
            CancellationToken cancellationToken)
        {
            return _readSuspectFunctionRepository.GetPlagiarizedFunctions(request);
        }
    }
}