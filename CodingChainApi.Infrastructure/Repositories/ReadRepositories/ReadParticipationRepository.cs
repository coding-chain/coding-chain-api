using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Participations;
using Application.Read.Participations.Handlers;
using Application.Read.ProgrammingLanguages;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

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
            var query = GetParticipationIncludeQueryable()
                .Where(ToPredicate(paginationQuery));
            if (paginationQuery.EndDateOrder is not null) query = GetOrderByQuery(query, paginationQuery);

            return await query
                .Select(p => ToParticipationNavigation(p))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<IList<ParticipationNavigation>> GetAllParticipationsByTeamAndTournamentId(Guid teamId,
            Guid tournamentId)
        {
            return await GetParticipationIncludeQueryable()
                .Where(p => !p.Deactivated
                            && !p.Tournament.IsDeleted && p.Tournament.IsPublished && p.Tournament.Id == tournamentId
                            && !p.Step.IsDeleted
                            && !p.Team.IsDeleted && p.Team.Id == teamId)
                .Select(p => ToParticipationNavigation(p))
                .ToListAsync();
        }

        public async Task<ParticipationNavigation?> GetOneParticipationNavigationById(Guid id)
        {
            var participation = await GetParticipationIncludeQueryable()
                .FirstOrDefaultAsync(p => !p.Deactivated && p.Id == id);
            return participation is null ? null : ToParticipationNavigation(participation);
        }

        public async Task<bool> ExistsById(Guid id)
        {
            return await GetParticipationIncludeQueryable()
                .AnyAsync(p => !p.Deactivated
                               && !p.Tournament.IsDeleted
                               && p.Tournament.IsPublished
                               && !p.Step.IsDeleted
                               && !p.Team.IsDeleted);
        }

        public async Task<bool> ParticipationExistsByTournamentStepTeamIds(Guid tournamentId, Guid stepId, Guid teamId)
        {
            return await GetParticipationIncludeQueryable()
                .AnyAsync(p => !p.Deactivated
                               && !p.Tournament.IsDeleted
                               && p.Tournament.IsPublished
                               && !p.Step.IsDeleted
                               && !p.Team.IsDeleted
                               && p.Tournament.Id == tournamentId
                               && p.Step.Id == stepId
                               && p.Team.Id == teamId);
        }

        public async Task<ProgrammingLanguageNavigation?> GetLanguageByParticipation(Guid participationId)
        {
            var participation = await _context.Participations
                .Include(p => p.Step)
                .ThenInclude(s => s.ProgrammingLanguage)
                .FirstOrDefaultAsync(p =>
                    p.Id == participationId && !p.Deactivated && !p.Step.IsDeleted &&
                    !p.Step.ProgrammingLanguage.IsDeleted);
            if (participation?.Step?.ProgrammingLanguage is null)
                return null;
            return ReadProgrammingLanguageRepository.ToProgrammingLanguageNavigation(participation.Step
                .ProgrammingLanguage);
        }

        private static IQueryable<Participation> GetOrderByQuery(IQueryable<Participation> participationQuery,
            GetAllParticipationNavigationPaginatedQuery paginationQuery)
        {
            if (paginationQuery.EndDateOrder == OrderEnum.Asc)
                return participationQuery.OrderBy(t => t.EndDate);
            return participationQuery.OrderByDescending(t => t.EndDate);
        }

        private static Expression<Func<Participation, bool>> ToPredicate(
            GetAllParticipationNavigationPaginatedQuery query)
        {
            return participation =>
                !participation.Deactivated
                && (query.EndTournamentStepFilter == null
                    || !participation.Step.IsDeleted
                    && participation.EndDate != null
                    && participation.Tournament.IsPublished
                    && !participation.Tournament.IsDeleted
                    && participation.Step.TournamentSteps
                        .Any(tournamentStep => tournamentStep.TournamentId == participation.Tournament.Id
                                               && tournamentStep.Step.Id == participation.Step.Id
                                               && tournamentStep.Order == participation.Tournament.TournamentSteps
                                                   .Max(tS => tS.Order)
                        )
                )
                && (query.TeamIdFilter == null ||
                    !participation.Team.IsDeleted && query.TeamIdFilter == participation.Team.Id)
                && (query.TournamentIdFilter == null || !participation.Tournament.IsDeleted &&
                    participation.Tournament.IsPublished && query.TournamentIdFilter == participation.Tournament.Id)
                && (query.StepIdFilter == null ||
                    !participation.Step.IsDeleted && query.StepIdFilter == participation.Step.Id);
        }

        private IQueryable<Participation> GetParticipationIncludeQueryable()
        {
            return _context.Participations
                .Include(p => p.Team)
                .Include(p => p.Tournament)
                .Include(p => p.Step)
                .Include(p => p.Functions);
        }

        private static ParticipationNavigation ToParticipationNavigation(Participation participation)
        {
            return new(
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
}