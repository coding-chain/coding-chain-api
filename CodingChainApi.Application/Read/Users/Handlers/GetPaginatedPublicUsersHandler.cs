using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Users.Handlers
{
    public record GetPublicUsersQuery : PaginationQueryBase, IRequest<IPagedList<PublicUser>>
    {
        public string? UsernameFilter { get; set; }
        public string? EmailFilter { get; set; }
        public IList<Guid>? WithoutIdsFilter { get; set; }

        public Guid? RightIdFilter { get; set; }
    }

    public class GetPaginatedPublicUsersHandler : IRequestHandler<GetPublicUsersQuery, IPagedList<PublicUser>>
    {
        private readonly IReadUserRepository _readUserRepository;

        public GetPaginatedPublicUsersHandler(IReadUserRepository readUserRepository)
        {
            _readUserRepository = readUserRepository;
        }

        public Task<IPagedList<PublicUser>> Handle(GetPublicUsersQuery request,
            CancellationToken cancellationToken)
        {
            return _readUserRepository.FindPaginatedPublicUsers(request);
        }
    }
}