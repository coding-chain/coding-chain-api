using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Teams;
using Application.Read.Tournaments;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Tournaments
                .Include(t => t.TournamentSteps)
                .ThenInclude(uT => uT.Step)
                .Where(t => !t.IsDeleted)
                .Select(t => new TournamentNavigation(t.Id, t.Name, t.Description, t.IsPublished, t.StartDate,
                    t.EndDate, ToStepsNavigation(t)))
                .FromPaginationQueryAsync(paginationQuery);
        }

        private static List<TournamentStepNavigation> ToStepsNavigation(Tournament t)
        {
            return t.TournamentSteps
                .Where(tS => !tS.Step.IsDeleted)
                .Select(tS => new TournamentStepNavigation(tS.StepId, tS.IsOptional, tS.Order))
                .ToList();
        }

        public async Task<TournamentNavigation?> GetOneTournamentNavigationByID(Guid id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.TournamentSteps)
                .ThenInclude(uT => uT.Step)
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);
            if (tournament is null) return null;
            return new TournamentNavigation(tournament.Id, tournament.Name, tournament.Description,
                tournament.IsPublished, tournament.StartDate, tournament.EndDate, ToStepsNavigation(tournament));
        }
    }
}