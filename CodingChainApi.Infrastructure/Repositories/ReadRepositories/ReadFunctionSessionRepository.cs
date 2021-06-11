using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.FunctionSessions;
using Application.Read.FunctionSessions.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Common.Pagination;
using CodingChainApi.Infrastructure.Models.Cache;
using CodingChainApi.Infrastructure.Services.Cache;
using Domain.Participations;
using Domain.ParticipationSessions;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadFunctionSessionRepository : IReadFunctionSessionRepository
    {
        private readonly ICache _cache;

        public ReadFunctionSessionRepository(ICache cache)
        {
            _cache = cache;
        }

        public async Task<IPagedList<FunctionSessionNavigation>> GetAllFunctionNavigationPaginated(
            GetParticipationSessionFunctionsPaginatedQuery paginationQuery)
        {
            var participation =
                await _cache.GetCache<ParticipationSession>(
                    new ParticipationId(paginationQuery.ParticipationId));
            if (participation is null)
                return PagedList<FunctionSessionNavigation>.Empty(paginationQuery.Page, paginationQuery.Size);
            var functions = participation.Functions
                .Where(f => paginationQuery.FunctionsIdsFilter is null ||
                            paginationQuery.FunctionsIdsFilter.Contains(f.Id))
                .Skip((paginationQuery.Page - 1) * paginationQuery.Size)
                .Take(paginationQuery.Size)
                .Select(ToFunctionSessionNavigation)
                .ToList();
            return new PagedList<FunctionSessionNavigation>(functions, participation.Functions.Count,
                paginationQuery.Page, paginationQuery.Size);
        }

        public async Task<FunctionSessionNavigation?> GetOneFunctionNavigationByIdAsync(Guid participationId,
            Guid functionId)
        {
            var participation =
                await _cache.GetCache<ParticipationSession>(participationId);
            var function = participation?.Functions.FirstOrDefault(f => f.Id == functionId);
            if (function is null) return null;
            return ToFunctionSessionNavigation(function);
        }

        public async Task<IList<FunctionSessionNavigation>> GetAllAsync(Guid participationId)
        {
            var participation =
                await _cache.GetCache<ParticipationSession>(new ParticipationId(participationId));
            var functions = participation?.Functions.Select(ToFunctionSessionNavigation).ToList() ??
                            new List<FunctionSessionNavigation>();
            return functions;
        }

        private static FunctionSessionNavigation ToFunctionSessionNavigation(Function function)
        {
            return new(
                function.Id,
                function.UserId,
                function.Code,
                function.LastModificationDate,
                function.Order
            );
        }
    }
}