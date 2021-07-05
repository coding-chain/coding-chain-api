using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.UserSessions;
using Application.Read.UserSessions.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Common.Pagination;
using CodingChainApi.Infrastructure.Models.Cache;
using CodingChainApi.Infrastructure.Services.Cache;
using Domain.Participations;
using Domain.ParticipationSessions;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadUserSessionRepository : IReadUserSessionRepository
    {
        private readonly ICache _cache;

        public ReadUserSessionRepository(ICache cache)
        {
            _cache = cache;
        }

        public async Task<IPagedList<UserSessionNavigation>> GetAllUserNavigationPaginated(
            GetParticipationSessionUsersPaginatedQuery paginationQuery)
        {
            var participation =
                await _cache.GetCache<ParticipationSession>(new ParticipationId(paginationQuery.ParticipationId));
            if (participation is null)
                return  PagedList<UserSessionNavigation>.Empty(paginationQuery.Page, paginationQuery.Size)
                    ;
            var functions = participation.Team.ConnectedUsers
                .Skip((paginationQuery.Page - 1) * paginationQuery.Size)
                .Take(paginationQuery.Size)
                .Select(ToUserSessionNavigation)
                .ToList();
            return new PagedList<UserSessionNavigation>(
                functions,
                participation.Team.ConnectedUsers.Count,
                paginationQuery.Page,
                paginationQuery.Size);
        }

        public async Task<UserSessionNavigation?> GetOneUserNavigationByIdAsync(Guid participationId, Guid userId)
        {
            var participation =
                await _cache.GetCache<ParticipationSession>(new ParticipationId(participationId));
            var user = participation?.Team.ConnectedUsers.FirstOrDefault(u => u.Id == userId);
            if (user is null) return null;
            return  ToUserSessionNavigation(user);
        }

        private static UserSessionNavigation ToUserSessionNavigation(ConnectedUser user)
        {
            return new(
                user.Id,
                user.IsAdmin
            );
        }
    }
}