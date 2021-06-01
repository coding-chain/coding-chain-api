using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.UserSessions.Handlers
{
    public record GetParticipationSessionUsersPaginatedQuery : PaginationQueryBase,
        IRequest<IPagedList<UserSessionNavigation>>
    {
        public Guid ParticipationId { get; set; }
    }
    public class GetParticipationSessionUsersPaginatedHandler : IRequestHandler<
        GetParticipationSessionUsersPaginatedQuery, IPagedList<UserSessionNavigation>>
    {
        private readonly IReadUserSessionRepository _readUserSessionRepository;

        public GetParticipationSessionUsersPaginatedHandler(
            IReadUserSessionRepository readUserSessionRepository)
        {
            _readUserSessionRepository = readUserSessionRepository;
        }

        public async Task<IPagedList<UserSessionNavigation>> Handle(
            GetParticipationSessionUsersPaginatedQuery request, CancellationToken cancellationToken)
        {
            return await _readUserSessionRepository.GetAllUserNavigationPaginated(request);
        }
    }
}