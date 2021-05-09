using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Teams;

namespace Application.Read.Contracts
{
    public interface IReadTeamRepository
    {
         public Task<IPagedList<TeamNavigation>> GetAllTeamNavigationPaginated(PaginationQueryBase paginationQuery);
         public Task<TeamNavigation?> GetOneTeamNavigationByIdAsync(Guid id);

         public Task<MemberNavigation?> GetOneMemberNavigationByIdAsync(Guid teamId, Guid userId);

    }
}