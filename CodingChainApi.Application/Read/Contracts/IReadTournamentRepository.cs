using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Teams;
using Application.Read.Tournaments;

namespace Application.Read.Contracts
{
    public interface IReadTournamentRepository
    {
        public Task<IPagedList<TournamentNavigation>> GetAllTournamentNavigationPaginated(PaginationQueryBase paginationQuery);
        public Task<TournamentNavigation?> GetOneTournamentNavigationByID(Guid id);
    }
}