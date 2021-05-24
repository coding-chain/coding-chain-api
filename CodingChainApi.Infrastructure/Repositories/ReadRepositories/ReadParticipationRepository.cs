using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Participations;
using Application.Read.Participations.Handlers;
using Application.Read.Teams.Handlers;
using Application.Read.Tournaments.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadParticipationRepository : IReadParticipationRepository
    {
        private readonly CodingChainContext _context;

        public ReadParticipationRepository(CodingChainContext context)
        {
            _context = context;
        }

        public async Task<IPagedList<ParticipationNavigation>> GetAllParticipationNavigationPaginated(
            GetAllParticipationNavigationPaginatedQuery paginationQuery)
        {
            return await GetParticipationIncludeQueryable()
                .Where(ToPredicate(paginationQuery))
                .Select(p => ToParticipationNavigation(p))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<ParticipationNavigation?> GetOneParticipationNavigationById(Guid id)
        {
            var participation = await GetParticipationIncludeQueryable()
                .FirstOrDefaultAsync(p => !p.Deactivated && p.Id == id);
            return participation is null ? null : ToParticipationNavigation(participation);
        }

        private static Expression<Func<Participation, bool>> ToPredicate(GetAllParticipationNavigationPaginatedQuery query) =>
            participation =>
                !participation.Deactivated
                && (query.TeamId == null || query.TeamId == participation.Team.Id)
                && (query.TournamentId == null || query.TournamentId == participation.Tournament.Id);
      
        private IIncludableQueryable<Participation, IList<Function>> GetParticipationIncludeQueryable()
        {
            return _context.Participations
                .Include(p => p.Team)
                .Include(p => p.Tournament)
                .Include(p => p.Step)
                .Include(p => p.Functions);
        }

        private static ParticipationNavigation ToParticipationNavigation(Participation participation) => new(
            participation.Id,
            participation.Team.Id,
            participation.Tournament.Id,
            participation.Step.Id,
            participation.StartDate,
            participation.EndDate,
            participation.CalculatedScore,
            participation.Functions.Where(f => !f.IsDeleted).Select(f => f.Id).ToList()
        );
    }
}