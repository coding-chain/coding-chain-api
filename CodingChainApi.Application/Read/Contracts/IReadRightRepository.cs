using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Rights;

namespace Application.Read.Contracts
{
    public interface IReadRightRepository
    {
        public Task<IPagedList<RightNavigation>> GetAllRightNavigationPaginated(PaginationQueryBase paginationQuery);
        public Task<RightNavigation?> GetOneRightNavigationById(Guid id);
    }
}