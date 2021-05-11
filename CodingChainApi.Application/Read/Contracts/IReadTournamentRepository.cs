using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Teams;
using Application.Read.Tournaments;
using Application.Read.Tournaments.Handlers;
using Application.Write.Tournaments;

namespace Application.Read.Contracts
{
    public interface IReadTournamentRepository
    {
        public Task<IPagedList<TournamentNavigation>> GetAllTournamentNavigationPaginated(GetTournamentNavigationPaginatedQuery paginationQuery);
        public Task<TournamentNavigation?> GetOneTournamentNavigationById(Guid id);
        
        public Task<IPagedList<TournamentStepNavigation>> GetAllTournamentStepNavigationPaginated(GetPaginatedTournamentStepNavigationQuery paginationQuery);
        public Task<TournamentStepNavigation?> GetOneTournamentStepNavigationByID(Guid tournamentId, Guid stepId);
        public Task<bool> TournamentExistsById(Guid tournamentId);
    }
}