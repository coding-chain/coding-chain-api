using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Tests;
using Application.Read.Tests.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadTestRepository : IReadTestRepository
    {
        private readonly CodingChainContext _context;

        public ReadTestRepository(CodingChainContext context)
        {
            _context = context;
        }

        private static TestNavigation ToTestNavigation(Test test) => new TestNavigation(
            test.Id,
            test.Step.Id,
            test.OutputValidator,
            test.InputGenerator,
            test.Score
        );

        public async Task<bool> TestExists(Guid testId)
        {
            return (await _context.Tests.FirstOrDefaultAsync(t => t.Id == testId)) is not null;
        }

        public async Task<TestNavigation?> GetOneTestNavigationByID(Guid testId)
        {
            var test = await GetTestIncludeQueryable()
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == testId);
            return test == null ? null : ToTestNavigation(test);
        }

        private static Expression<Func<Test, bool>> ToExpression(GetPaginatedTestNavigationQuery query) =>
            test => !test.IsDeleted && (query.StepId == null || query.StepId == test.Step.Id);
        public async Task<IPagedList<TestNavigation>> GetPaginatedTestNavigation(GetPaginatedTestNavigationQuery query)
        {
            return await GetTestIncludeQueryable()
                .Where(ToExpression(query))
                .Select(t => ToTestNavigation(t))
                .FromPaginationQueryAsync(query);
        }
        
        public async Task<IList<TestNavigation>> GetAllTestNavigationByStepId(Guid stepId)
        {
            return await GetTestIncludeQueryable()
                .Where(t => !t.Step.IsDeleted && t.Step.Id == stepId  )
                .Select(t => ToTestNavigation(t))
                .ToListAsync();
        }

        private IIncludableQueryable<Test, Step> GetTestIncludeQueryable()
        {
            return _context.Tests
                .Include(t => t.Step);
        }
    }
}