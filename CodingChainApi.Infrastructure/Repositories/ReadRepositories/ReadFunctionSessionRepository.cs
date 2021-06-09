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
                _cache.GetCache<ParticipationSessionAggregate>(new ParticipationId(paginationQuery.ParticipationId));
            if (participation is null)
                return await PagedList<FunctionSessionNavigation>.Empty(paginationQuery.Page, paginationQuery.Size)
                    .ToTask();
            var functions = participation.Functions
                .Where(f => paginationQuery.FunctionsIdsFilter is null ||
                            paginationQuery.FunctionsIdsFilter.Contains(f.Id.Value))
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
                _cache.GetCache<ParticipationSessionAggregate>(new ParticipationId(participationId));
            var function = participation?.Functions.FirstOrDefault(f => f.Id.Value == functionId);
            if (function is null) return null;
            return await ToFunctionSessionNavigation(function).ToTask();
        }

        public async Task<IList<FunctionSessionNavigation>> GetAllAsync(Guid participationId)
        {
            var participation =
                _cache.GetCache<ParticipationSessionAggregate>(new ParticipationId(participationId));
            var functions = participation?.Functions.Select(ToFunctionSessionNavigation).ToList() ??
                            new List<FunctionSessionNavigation>();
            return await functions.ToTask();
        }

        private static FunctionSessionNavigation ToFunctionSessionNavigation(FunctionEntity function)
        {
            return new(
                function.Id.Value,
                function.UserId.Value,
                function.Code,
                function.LastModificationDate,
                function.Order
            );
        }
    }
}