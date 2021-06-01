using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Participations;
using Application.Read.Participations.Handlers;
using Application.Read.Teams.Handlers;

namespace Application.Read.Contracts
{
    public interface IReadParticipationRepository
    {
        public Task<IPagedList<ParticipationNavigation>> GetAllParticipationNavigationPaginated(
            GetAllParticipationNavigationPaginatedQuery paginationQuery);

        public Task<ParticipationNavigation?> GetOneParticipationNavigationById(Guid id);
        public Task<bool> ExistsById(Guid id);
        public Task<bool> ParticipationExistsByTournamentStepTeamIds(Guid tournamentId, Guid stepId, Guid teamId);
    }
}