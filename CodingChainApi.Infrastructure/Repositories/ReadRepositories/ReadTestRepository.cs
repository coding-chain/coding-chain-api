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
using CodingChainApi.Infrastructure.Common.Pagination;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using CodingChainApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadTestRepository : IReadTestRepository
    {
        private readonly CodingChainContext _context;
        private readonly IFunctionTypeParserService _functionTypeParser;

        public ReadTestRepository(CodingChainContext context, IFunctionTypeParserService functionTypeParser)
        {
            _context = context;
            _functionTypeParser = functionTypeParser;
        }

        private static TestNavigation ToTestNavigation(Test test) => new TestNavigation(
            test.Id,
            test.Step.Id,
            test.Name,
            test.OutputValidator,
            test.InputGenerator,
            test.Score
        );

        private PublicTestNavigation ToPublicTestNavigation(Test test)
        {
            var inType = _functionTypeParser.GetReturnType(test.InputGenerator, test.Step.ProgrammingLanguage.Name);
            var outType = _functionTypeParser.GetInputType(test.OutputValidator, test.Step.ProgrammingLanguage.Name);
            return new PublicTestNavigation(
                test.Id,
                test.Step.Id,
                test.Name,
                inType,
                outType
            );
        }


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
        private static Expression<Func<Test, bool>> ToExpression(GetPaginatedPublicTestNavigationQuery query) =>
            test => !test.IsDeleted && (query.StepId == null || query.StepId == test.Step.Id);
        public async Task<IPagedList<TestNavigation>> GetPaginatedTestNavigation(GetPaginatedTestNavigationQuery query)
        {
            return await GetTestIncludeQueryable()
                .Where(ToExpression(query))
                .Select(t => ToTestNavigation(t))
                .FromPaginationQueryAsync(query);
        }

        public async Task<IPagedList<PublicTestNavigation>> GetPaginatedPublicTestNavigation(
            GetPaginatedPublicTestNavigationQuery query)
        {
            var tests = await GetTestIncludeQueryable()
                .Include(t => t.Step)
                .ThenInclude(s=> s.ProgrammingLanguage)
                .Where(ToExpression(query))
                .FromPaginationQueryAsync(query);
            return PagedList<PublicTestNavigation>.From(tests.Select(ToPublicTestNavigation).ToList(), tests);
        }


        public async Task<IList<TestNavigation>> GetAllTestNavigationByStepId(Guid stepId)
        {
            return await GetTestIncludeQueryable()
                .Where(t => !t.Step.IsDeleted && t.Step.Id == stepId)
                .Select(t => ToTestNavigation(t))
                .ToListAsync();
        }

        private IQueryable<Test> GetTestIncludeQueryable()
        {
            return _context.Tests
                .Include(t => t.Step);
        }
    }
}