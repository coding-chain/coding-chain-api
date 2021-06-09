using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.FunctionSessions.Handlers
{
    public record GetParticipationSessionFunctionsPaginatedQuery : PaginationQueryBase,
        IRequest<IPagedList<FunctionSessionNavigation>>
    {
        public Guid ParticipationId { get; set; }
        public IList<Guid>? FunctionsIdsFilter { get; set; }
    }

    public class GetParticipationSessionFunctionsPaginatedHandler : IRequestHandler<
        GetParticipationSessionFunctionsPaginatedQuery, IPagedList<FunctionSessionNavigation>>
    {
        private readonly IReadFunctionSessionRepository _readFunctionSessionRepository;

        public GetParticipationSessionFunctionsPaginatedHandler(
            IReadFunctionSessionRepository readFunctionSessionRepository)
        {
            _readFunctionSessionRepository = readFunctionSessionRepository;
        }

        public async Task<IPagedList<FunctionSessionNavigation>> Handle(
            GetParticipationSessionFunctionsPaginatedQuery request, CancellationToken cancellationToken)
        {
            return await _readFunctionSessionRepository.GetAllFunctionNavigationPaginated(request);
        }
    }
}