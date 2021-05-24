using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Participations;
using Application.Read.Teams;
using Application.Read.Teams.Handlers;
using Application.Read.Tournaments;

namespace Application.Read.Contracts
{
    public interface IReadTeamRepository
    {
         public Task<IPagedList<TeamNavigation>> GetAllTeamNavigationPaginated(GetTeamNavigationPaginatedQuery paginationQuery);
         public Task<TeamNavigation?> GetOneTeamNavigationByIdAsync(Guid id);

         public Task<MemberNavigation?> GetOneMemberNavigationByIdAsync(Guid teamId, Guid userId);

         public Task<IList<Guid>> GetTeamMembersIds(Guid teamId);

         public Task<bool> TeamExistsById(Guid id);

         public Task<IPagedList<MemberNavigation>> GetAllMembersNavigationPaginated(GetPaginatedTeamMembersQuery query);

    }
}