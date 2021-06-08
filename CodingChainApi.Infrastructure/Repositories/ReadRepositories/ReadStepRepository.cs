using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Steps;
using Application.Read.Steps.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadStepRepository : IReadStepRepository
    {
        private readonly CodingChainContext _context;

        public ReadStepRepository(CodingChainContext context)
        {
            _context = context;
        }

        public Task<bool> StepExistsById(Guid stepId)
        {
            return _context.Steps.AnyAsync(s => s.Id == stepId && !s.IsDeleted);
        }

        public async Task<bool> StepExistsByIds(Guid[] stepIds)
        {
            var res = await Task.WhenAll(stepIds.Select(async stepId =>
                await _context.Steps.AnyAsync(s => !s.IsDeleted && s.Id == stepId)));
            return res.Length == stepIds.Length;
        }

        public async Task<IPagedList<StepNavigation>> GetAllStepNavigationPaginated(
            GetPaginatedStepNavigationQuery paginationQuery)
        {
            var query = GetStepIncludeQueryable()
                .Where(FromQuery(paginationQuery));
            if (paginationQuery.IsPublishedFilter is not null)
                query = query.Where(IsPublishedFromQuery(paginationQuery));

            query = GetOrderByQuery(query, paginationQuery);
            return await query.Select(s => ToStepNavigation(s))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<StepNavigation?> GetOneStepNavigationById(Guid id)
        {
            var step = await GetStepIncludeQueryable()
                .FirstOrDefaultAsync(s => !s.IsDeleted && s.Id == id);
            return step is null ? null : ToStepNavigation(step);
        }

        private static StepNavigation ToStepNavigation(Step step)
        {
            return new(
                step.Id,
                step.ProgrammingLanguage.Id,
                step.Name,
                step.Description,
                step.MinFunctionsCount,
                step.MaxFunctionsCount,
                step.Score,
                step.Difficulty,
                step.HeaderCode,
                step.IsPublished,
                step.TestsIds,
                step.TournamentsIds,
                step.ActiveParticipationsIds);
        }

        public static Expression<Func<Step, bool>> IsPublishedFromQuery(GetPaginatedStepNavigationQuery query)
        {
            return step => query.IsPublishedFilter == null || query.IsPublishedFilter.Value
                ? step.TournamentSteps.Any(tS => !tS.Tournament.IsDeleted && tS.Tournament.IsPublished)
                : !step.TournamentSteps.Any(tS => !tS.Tournament.IsDeleted && tS.Tournament.IsPublished);
        }

        public static Expression<Func<Step, bool>> FromQuery(GetPaginatedStepNavigationQuery query)
        {
            return step => !step.IsDeleted
                           && (query.NameFilter == null || step.Name.ToLowerInvariant()
                               .Contains(query.NameFilter.ToLowerInvariant()))
                           && (query.LanguageIdFilter == null || !step.ProgrammingLanguage.IsDeleted &&
                               step.ProgrammingLanguage.Id == query.LanguageIdFilter)
                           && (query.WithoutIdsFilter == null || query.WithoutIdsFilter.All(id => id != step.Id));
        }

        private static IQueryable<Step> GetOrderByQuery(IQueryable<Step> stepQuery,
            GetPaginatedStepNavigationQuery paginationQuery)
        {
            if (paginationQuery.NameOrder == OrderEnum.Asc)
                return stepQuery.OrderBy(t => t.Name);
            return stepQuery.OrderByDescending(t => t.Name);
        }

        private IQueryable<Step> GetStepIncludeQueryable()
        {
            return _context.Steps
                .Include(s => s.Tests)
                .Include(s => s.ProgrammingLanguage)
                .Include(s => s.Participations)
                .ThenInclude<Step, Participation, Team>(p => p.Team)
                .Include(s => s.TournamentSteps)
                .ThenInclude<Step, TournamentStep, Tournament>(tS => tS.Tournament);
        }
    }
}