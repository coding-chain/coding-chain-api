using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Teams;
using Application.Read.Tournaments;
using Application.Read.Tournaments.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
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
            PaginationQueryBase paginationQuery)
        {
            return await GetTournamentIncludeQueryable()
                .Where(t => !t.IsDeleted)
                .Select(t => new TournamentNavigation(t.Id, t.Name, t.Description, t.IsPublished, t.StartDate,
                    t.EndDate, ToStepsIds(t)))
                .FromPaginationQueryAsync(paginationQuery);
        }

        private static List<Guid> ToStepsIds(Tournament t)
        {
            return t.TournamentSteps
                .Where(tS => !tS.Step.IsDeleted)
                .Select(tS => tS.Id)
                .ToList();
        }

        public async Task<TournamentNavigation?> GetOneTournamentNavigationById(Guid id)
        {
            var tournament = await GetTournamentIncludeQueryable()
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);
            if (tournament is null) return null;
            return new TournamentNavigation(tournament.Id, tournament.Name, tournament.Description,
                tournament.IsPublished, tournament.StartDate, tournament.EndDate, ToStepsIds(tournament));
        }

        private IIncludableQueryable<Tournament, Step> GetTournamentIncludeQueryable()
        {
            return _context.Tournaments
                .Include(t => t.TournamentSteps)
                .ThenInclude(uT => uT.Step);
        }

        private static TournamentStepNavigation ToTournamentStepNavigation(TournamentStep tournamentStep) =>
            new(
                tournamentStep.StepId,
                tournamentStep.TournamentId,
                tournamentStep.IsOptional,
                tournamentStep.Order
            );

        public async Task<IPagedList<TournamentStepNavigation>> GetAllTournamentStepNavigationPaginated(
            GetPaginatedTournamentStepNavigationQuery paginationQuery)
        {
            return await GetTournamentStepIncludeQueryable()
                .Where(tournamentStep => !tournamentStep.Tournament.IsDeleted && !tournamentStep.Step.IsDeleted &&
                                         tournamentStep.TournamentId == paginationQuery.TournamentId)
                .Select(tournamentStep => ToTournamentStepNavigation(tournamentStep))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<TournamentStepNavigation?> GetOneTournamentStepNavigationByID(Guid tournamentId, Guid stepId)
        {
            var tournamentStep = await GetTournamentStepIncludeQueryable()
                .FirstOrDefaultAsync(tS => !tS.Tournament.IsDeleted && !tS.Step.IsDeleted && tS.TournamentId == tournamentId);
            return tournamentStep is null ? null : ToTournamentStepNavigation(tournamentStep);
        }

        public Task<bool> TournamentExistsById(Guid tournamentId)
        {
            return _context.Tournaments.AnyAsync(t => !t.IsDeleted && t.Id == tournamentId);
        }

        private IIncludableQueryable<TournamentStep, Step> GetTournamentStepIncludeQueryable()
        {
            return _context.TournamentSteps
                .Include(tS => tS.Tournament)
                .Include(tS => tS.Step);
        }
    }
}