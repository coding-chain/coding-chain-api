using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.UserSessions;
using Application.Read.UserSessions.Handlers;

namespace Application.Read.Contracts
{
    public interface IReadUserSessionRepository
    {
        public Task<IPagedList<UserSessionNavigation>> GetAllUserNavigationPaginated(
            GetParticipationSessionUsersPaginatedQuery paginationQuery);

        public Task<UserSessionNavigation?> GetOneUserNavigationByIdAsync(Guid participationId, Guid userId);
    }
}