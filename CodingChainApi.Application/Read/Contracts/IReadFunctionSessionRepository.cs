using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.FunctionSessions;
using Application.Read.FunctionSessions.Handlers;

namespace Application.Read.Contracts
{
    public interface IReadFunctionSessionRepository
    {
        public Task<IPagedList<FunctionSessionNavigation>> GetAllFunctionNavigationPaginated(
            GetParticipationSessionFunctionsPaginatedQuery paginationQuery);

        public Task<FunctionSessionNavigation?>
            GetOneFunctionNavigationByIdAsync(Guid participationId, Guid functionId);

        public Task<IList<FunctionSessionNavigation>> GetAllAsync(Guid participationId);
    }
}