using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Steps;
using Application.Read.Tests;
using Application.Read.Tests.Handlers;

namespace Application.Read.Contracts
{
    public interface IReadTestRepository
    {
        public Task<bool> TestExists(Guid testId);
        public Task<TestNavigation?> GetOneTestNavigationByID( Guid testId);
        public Task<IPagedList<TestNavigation>> GetPaginatedTestNavigation(GetPaginatedTestNavigationQuery query);
    }
}