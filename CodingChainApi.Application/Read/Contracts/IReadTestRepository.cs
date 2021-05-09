using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Steps;
using Application.Read.Tests;

namespace Application.Read.Contracts
{
    public interface IReadTestRepository
    {
        public Task<TestNavigation?> GetOneTestNavigationByID( Guid testId);
        public Task<IPagedList<TestNavigation>> GetPaginatedTestNavigation(PaginationQueryBase query);
    }
}