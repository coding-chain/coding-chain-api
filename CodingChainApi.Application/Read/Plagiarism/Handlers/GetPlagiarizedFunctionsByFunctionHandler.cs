using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Plagiarism.Handlers
{
    public record GetLastPlagiarizedFunctionsByFunctionQuery(Guid FunctionId) : PaginationQueryBase,
        IRequest<IPagedList<FunctionCodeNavigation>>;

    public class GetPlagiarizedFunctionsByFunctionHandler : IRequestHandler<GetLastPlagiarizedFunctionsByFunctionQuery,
        IPagedList<FunctionCodeNavigation>>
    {
        private IReadFunctionRepository _readFunctionRepository;

        public GetPlagiarizedFunctionsByFunctionHandler(IReadFunctionRepository readFunctionRepository)
        {
            _readFunctionRepository = readFunctionRepository;
        }

        public Task<IPagedList<FunctionCodeNavigation>> Handle(GetLastPlagiarizedFunctionsByFunctionQuery request,
            CancellationToken cancellationToken)
        {
            return _readFunctionRepository.GetPlagiarizedFunctions(request);
        }
    }
}