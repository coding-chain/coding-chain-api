using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Teams.Handlers
{
    public record GetPaginatedTeamMembersQuery:  PaginationQueryBase, IRequest<IPagedList<MemberNavigation>>
    {
       public Guid TeamId { get; set; } 
    }
    public class GetPaginatedTeamMembersHandler: IRequestHandler<GetPaginatedTeamMembersQuery, IPagedList<MemberNavigation>>
    {
        private readonly IReadTeamRepository _readTeamRepository;

        public GetPaginatedTeamMembersHandler(IReadTeamRepository readTeamRepository)
        {
            _readTeamRepository = readTeamRepository;
        }

        public Task<IPagedList<MemberNavigation>> Handle(GetPaginatedTeamMembersQuery request, CancellationToken cancellationToken)
        {
            return this._readTeamRepository.GetAllMembersNavigationPaginated(request);
        }
    }
}