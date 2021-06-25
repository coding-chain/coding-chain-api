using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Domain.Users;
using RightNavigation = Application.Read.Rights.RightNavigation;

namespace Application.Read.Contracts
{
    public interface IReadRightRepository
    {
        public Task<IPagedList<RightNavigation>> GetAllRightNavigationPaginated(PaginationQueryBase paginationQuery);
        public Task<RightNavigation?> GetOneRightNavigationById(Guid id);
        public Task<RightNavigation?> GetOneRightNavigationByName(RightEnum name);
    }
}