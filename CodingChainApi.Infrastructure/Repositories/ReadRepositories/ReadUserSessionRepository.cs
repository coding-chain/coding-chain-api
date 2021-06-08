using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.UserSessions;
using Application.Read.UserSessions.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Common.Pagination;
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
                _cache.GetCache<ParticipationSessionAggregate>(new ParticipationId(paginationQuery.ParticipationId));
            if (participation is null)
                return await PagedList<UserSessionNavigation>.Empty(paginationQuery.Page, paginationQuery.Size)
                    .ToTask();
            var functions = participation.ConnectedTeam.ConnectedUserEntities
                .Skip((paginationQuery.Page - 1) * paginationQuery.Size)
                .Take(paginationQuery.Size)
                .Select(ToUserSessionNavigation)
                .ToList();
            return new PagedList<UserSessionNavigation>(
                functions,
                participation.ConnectedTeam.ConnectedUserEntities.Count,
                paginationQuery.Page,
                paginationQuery.Size);
        }

        public async Task<UserSessionNavigation?> GetOneUserNavigationByIdAsync(Guid participationId, Guid userId)
        {
            var participation =
                _cache.GetCache<ParticipationSessionAggregate>(new ParticipationId(participationId));
            var user = participation?.ConnectedTeam.ConnectedUserEntities.FirstOrDefault(u => u.Id.Value == userId);
            if (user is null) return null;
            return await ToUserSessionNavigation(user).ToTask();
        }

        private static UserSessionNavigation ToUserSessionNavigation(ConnectedUserEntity user)
        {
            return new(
                user.Id.Value,
                user.IsAdmin
            );
        }
    }
}