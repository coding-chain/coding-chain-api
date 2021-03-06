using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Tournaments;
using Application.Read.Tournaments.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using CodingChainApi.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadTournamentRepository : IReadTournamentRepository
    {
        private readonly CodingChainContext _context;

        public ReadTournamentRepository(CodingChainContext context)
        {
            _context = context;
        }


        public async Task<IPagedList<TournamentNavigation>> GetAllTournamentNavigationPaginated(
            GetTournamentNavigationPaginatedQuery paginationQuery)
        {
            var query = GetTournamentIncludeQueryable()
                .ThenInclude(s => s.ProgrammingLanguage)
                .Include(t => t.Participations)
                .ThenInclude<Tournament, Participation, Team>(p => p.Team)
                .ThenInclude(t => t.UserTeams)
                .ThenInclude<Tournament, UserTeam, User>(uT => uT.User)
                .Where(ToPredicate(paginationQuery));
            query = GetOrderByQuery(query, paginationQuery);
            return await query
                .Select(t => ToTournamentNavigation(t))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<TournamentNavigation?> GetOneTournamentNavigationById(Guid id)
        {
            var tournament = await GetTournamentIncludeQueryable()
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);
            return tournament is null ? null : ToTournamentNavigation(tournament);
        }


        public async Task<IPagedList<TournamentStepNavigation>> GetAllTournamentStepNavigationPaginated(
            GetPaginatedTournamentStepNavigationQuery paginationQuery)
        {
            return await GetTournamentStepIncludeQueryable()
                .Where(tournamentStep => !tournamentStep.Tournament.IsDeleted && !tournamentStep.Step.IsDeleted &&
                                         tournamentStep.TournamentId == paginationQuery.TournamentId)
                .Select(tournamentStep => ToTournamentStepNavigation(tournamentStep))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<IList<TournamentStepNavigation>> GetAllTournamentStepNavigationByTournamentId(
            Guid tournamentId)
        {
            return await GetTournamentStepIncludeQueryable()
                .Where(tournamentStep => !tournamentStep.Tournament.IsDeleted && !tournamentStep.Step.IsDeleted &&
                                         tournamentStep.TournamentId == tournamentId)
                .Select(tournamentStep => ToTournamentStepNavigation(tournamentStep))
                .ToListAsync();
        }


        public async Task<TournamentStepNavigation?> GetOneTournamentStepNavigationByID(Guid tournamentId, Guid stepId)
        {
            var tournamentStep = await GetTournamentStepIncludeQueryable()
                .FirstOrDefaultAsync(tS =>
                    !tS.Tournament.IsDeleted && !tS.Step.IsDeleted && tS.TournamentId == tournamentId);
            return tournamentStep is null ? null : ToTournamentStepNavigation(tournamentStep);
        }

        public Task<bool> TournamentExistsById(Guid tournamentId)
        {
            return _context.Tournaments.AnyAsync(t => !t.IsDeleted && t.Id == tournamentId);
        }

        private static Expression<Func<Tournament, bool>> ToPredicate(GetTournamentNavigationPaginatedQuery query)
        {
            return tournament =>
                !tournament.IsDeleted
                && (query.TeamId == null || tournament.Participations.Any(p =>
                    !p.Deactivated && p.Team.Id == query.TeamId && !p.Team.IsDeleted && !p.Step.IsDeleted))
                && (query.IsPublishedFilter == null || tournament.IsPublished == query.IsPublishedFilter)
                && (query.ParticipantIdFilter == null || tournament.Participations.Any(p =>
                    !p.Deactivated && !p.Team.IsDeleted && p.Team.UserTeams.Any(uT =>
                        uT.LeaveDate == null && !uT.User.IsDeleted && uT.User.Id == query.ParticipantIdFilter)))
                && (query.LanguageIdFilter == null || tournament.TournamentSteps.Any(tS =>
                    !tS.Step.IsDeleted && tS.Step.ProgrammingLanguage.Id == query.LanguageIdFilter))
                && (query.NameFilter == null || tournament.Name.Contains(query.NameFilter));
        }

        private static IQueryable<Tournament> GetOrderByQuery(IQueryable<Tournament> tournamentQuery,
            GetTournamentNavigationPaginatedQuery paginationQuery)
        {
            if (paginationQuery.NameOrder == OrderEnum.Asc)
                return tournamentQuery.OrderBy(t => t.Name);
            return tournamentQuery.OrderByDescending(t => t.Name);
        }


        public static TournamentNavigation ToTournamentNavigation(Tournament tournament)
        {
            return new(
                tournament.Id,
                tournament.Name,
                tournament.Description,
                tournament.IsPublished,
                tournament.StartDate,
                tournament.EndDate,
                tournament.StepsIds,
                tournament.ParticipationsIds
            );
        }

        private IIncludableQueryable<Tournament, Step> GetTournamentIncludeQueryable()
        {
            return _context.Tournaments
                .Include(t => t.TournamentSteps)
                .ThenInclude<Tournament, TournamentStep, Step>(uT => uT.Step);
        }

        private static TournamentStepNavigation ToTournamentStepNavigation(TournamentStep tournamentStep)
        {
            return new(
                tournamentStep.StepId,
                tournamentStep.TournamentId,
                tournamentStep.IsOptional,
                tournamentStep.Order,
                tournamentStep.Step.ProgrammingLanguage.Id,
                tournamentStep.Step.Name,
                tournamentStep.Step.Description,
                tournamentStep.Step.MinFunctionsCount,
                tournamentStep.Step.MaxFunctionsCount,
                tournamentStep.Step.Score,
                tournamentStep.Step.Difficulty,
                tournamentStep.Step.HeaderCode,
                tournamentStep.Step.IsPublished,
                tournamentStep.Step.TestsIds,
                tournamentStep.Step.ActiveParticipationsIds,
                tournamentStep.Step.TournamentsIds
            );
        }

        private IIncludableQueryable<TournamentStep, Tournament> GetTournamentStepIncludeQueryable()
        {
            return _context.TournamentSteps
                .Include(tS => tS.Tournament)
                .Include(tS => tS.Step)
                .ThenInclude(s => s.ProgrammingLanguage)
                .Include(tS => tS.Step)
                .ThenInclude(s => s.Tests)
                .Include(tS => tS.Step)
                .ThenInclude(s => s.Participations)
                .ThenInclude<TournamentStep, Participation, Team>(p => p.Team)
                .Include(tS => tS.Step)
                .ThenInclude(s => s.TournamentSteps)
                .ThenInclude<TournamentStep, TournamentStep, Tournament>(tS => tS.Tournament);
        }
    }
}