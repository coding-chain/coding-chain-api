using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Participations;
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
            PaginationQueryBase paginationQuery)
        {
            return await GetParticipationIncludeQueryable()
                .Select(p => ToParticipationNavigation(p))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<ParticipationNavigation?> GetOneParticipationNavigationById(Guid id)
        {
            var participation = await GetParticipationIncludeQueryable()
                .FirstOrDefaultAsync(p => p.Id == id);
            return participation is null ? null : ToParticipationNavigation(participation);
        }

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