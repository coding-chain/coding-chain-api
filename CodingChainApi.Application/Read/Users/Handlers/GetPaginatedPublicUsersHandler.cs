using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Users.Handlers
{
    public record GetPaginatedPublicUsersQuery : PaginationQueryBase, IRequest<IPagedList<PublicUser>>;
    public class GetPaginatedPublicUsersHandler: IRequestHandler<GetPaginatedPublicUsersQuery, IPagedList<PublicUser>>
    {
        private readonly IReadUserRepository _readUserRepository;

        public GetPaginatedPublicUsersHandler(IReadUserRepository readUserRepository)
        {
            _readUserRepository = readUserRepository;
        }

        public Task<IPagedList<PublicUser>> Handle(GetPaginatedPublicUsersQuery request, CancellationToken cancellationToken)
        {
            return _readUserRepository.FindPaginatedPublicUsers(request);
        }
    }
}