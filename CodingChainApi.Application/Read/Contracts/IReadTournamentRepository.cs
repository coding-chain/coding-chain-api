using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Tournaments;
using Application.Read.Tournaments.Handlers;

namespace Application.Read.Contracts
{
    public interface IReadTournamentRepository
    {
        public Task<IPagedList<TournamentNavigation>> GetAllTournamentNavigationPaginated(
            GetTournamentNavigationPaginatedQuery paginationQuery);

        public Task<TournamentNavigation?> GetOneTournamentNavigationById(Guid id);

        public Task<IPagedList<TournamentStepNavigation>> GetAllTournamentStepNavigationPaginated(
            GetPaginatedTournamentStepNavigationQuery paginationQuery);

        public Task<TournamentStepNavigation?> GetOneTournamentStepNavigationByID(Guid tournamentId, Guid stepId);
        public Task<bool> TournamentExistsById(Guid tournamentId);

        Task<IList<TournamentStepNavigation>> GetAllTournamentStepNavigationByTournamentId(
            Guid tournamentId);

    }
}