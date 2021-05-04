using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Users;
using Domain.Users;

namespace Application.Read.Contracts
{
    public interface IReadUserRepository
    {
        public Task<bool> UserExistsByEmailAsync(string email);
        public Task<bool> UserExistsByIdAsync(Guid id);
        public Task<Guid?> FindUserIdByEmail(string email);

        public Task<PublicUser?> FindPublicUserById(Guid id);
        public Task<IPagedList<PublicUser>> FindPaginatedPublicUsers(PaginationQueryBase query);
    }
}