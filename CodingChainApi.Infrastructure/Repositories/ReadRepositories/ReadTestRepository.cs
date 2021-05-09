using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Tests;
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

        public async Task<TestNavigation?> GetOneTestNavigationByID(Guid testId)
        {
            var test = await GetTestIncludeQueryable()
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == testId);
            return test == null ? null : ToTestNavigation(test);
        }

        public async Task<IPagedList<TestNavigation>> GetPaginatedTestNavigation(PaginationQueryBase query)
        {
            return await GetTestIncludeQueryable()
                .Where(t => t.IsDeleted)
                .Select(t => ToTestNavigation(t))
                .FromPaginationQueryAsync(query);
        }

        private IIncludableQueryable<Test, Step> GetTestIncludeQueryable()
        {
            return _context.Tests
                .Include(t => t.Step);
        }
    }
}