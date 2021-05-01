using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Languages.Handlers
{
    public record GetAllProgrammingLanguagesPaginatedQuery: IPaginationQuery, IRequest<IPagedList<ProgrammingLanguageNavigation>>
    {
        public int Page { get; set; }
        public int Size { get; set; }
    }

    public class GetAllProgrammingLanguagesPaginatedHandler: IRequestHandler<GetAllProgrammingLanguagesPaginatedQuery, IPagedList<ProgrammingLanguageNavigation>>
    {
        private readonly IReadProgrammingLanguageRepository _readProgrammingLanguageRepository;

        public GetAllProgrammingLanguagesPaginatedHandler(IReadProgrammingLanguageRepository readProgrammingLanguageRepository)
        {
            _readProgrammingLanguageRepository = readProgrammingLanguageRepository;
        }

        public Task<IPagedList<ProgrammingLanguageNavigation>> Handle(GetAllProgrammingLanguagesPaginatedQuery request, CancellationToken cancellationToken)
        {
            return _readProgrammingLanguageRepository.GetAllLanguageNavigationPaginated(request);
        }
    }
}