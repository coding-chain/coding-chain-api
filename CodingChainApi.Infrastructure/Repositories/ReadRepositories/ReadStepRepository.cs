using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Steps;
using Application.Read.Steps.Handlers;
using Application.Read.Tournaments;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadStepRepository : IReadStepRepository
    {
        private readonly CodingChainContext _context;

        public ReadStepRepository(CodingChainContext context)
        {
            _context = context;
        }

        private static StepNavigation ToStepNavigation(Step step) => new(
            step.Id,
            step.ProgrammingLanguage.Id,
            step.Name,
            step.Description,
            step.MinFunctionsCount,
            step.MaxFunctionsCount,
            step.Score,
            step.Difficulty,
            step.HeaderCode,
            step.TestsIds,
            step.TournamentsIds,
            step.ActiveParticipationsIds);

        public Task<bool> StepExistsById(Guid stepId)
        {
            return _context.Steps.AnyAsync(s => s.Id == stepId && !s.IsDeleted);
        }
        
        public async Task<bool> StepExistsByIds(Guid[] stepIds)
        {
            var res = await Task.WhenAll(stepIds.Select(async stepId => await _context.Steps.AnyAsync(s => !s.IsDeleted && s.Id == stepId)));
            return res.Length == stepIds.Length;
        }
        
        public static Expression<Func<Step,bool>> FromQuery(GetPaginatedStepNavigationQuery query)
        {
            return step => !step.IsDeleted && (query.IsPublished == null || step.IsPublished == query.IsPublished);
        }   

        public async Task<IPagedList<StepNavigation>> GetAllStepNavigationPaginated(
            GetPaginatedStepNavigationQuery paginationQuery)
        {
            return await  GetStepIncludeQueryable()
                .ThenInclude(tS => tS.Tournament)
                .Where(FromQuery(paginationQuery))
                .Select(s => ToStepNavigation(s))
                .FromPaginationQueryAsync(paginationQuery);
        }

        private IIncludableQueryable<Step, IList<TournamentStep>> GetStepIncludeQueryable()
        {
            return _context.Steps
                .Include(s => s.Tests)
                .Include(s => s.ProgrammingLanguage)
                .Include(s => s.Participations)
                .Include(s => s.TournamentSteps);
        }

        public async Task<StepNavigation?> GetOneStepNavigationById(Guid id)
        {
            var step = await GetStepIncludeQueryable()
                .FirstOrDefaultAsync(s => !s.IsDeleted && s.Id == id);
            return step is null ? null : ToStepNavigation(step);
        }

    }
}