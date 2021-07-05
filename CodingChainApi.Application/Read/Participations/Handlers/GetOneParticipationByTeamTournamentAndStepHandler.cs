using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Participations.Handlers
{
    public record GetOneParticipationByTeamTournamentAndStepQuery(Guid TeamId, Guid TournamentId, Guid StepId) :
        IRequest<ParticipationNavigation>;

    public class GetOneParticipationByTeamTournamentAndStepHandler : IRequestHandler<
        GetOneParticipationByTeamTournamentAndStepQuery, ParticipationNavigation>
    {
        private readonly IReadParticipationRepository _readParticipationRepository;

        public GetOneParticipationByTeamTournamentAndStepHandler(
            IReadParticipationRepository readParticipationRepository)
        {
            _readParticipationRepository = readParticipationRepository;
        }

        public async Task<ParticipationNavigation> Handle(GetOneParticipationByTeamTournamentAndStepQuery request,
            CancellationToken cancellationToken)
        {
            var participations = await _readParticipationRepository.GetAllParticipationNavigationPaginated(
                new GetAllParticipationNavigationPaginatedQuery
                {
                    StepIdFilter = request.StepId,
                    TeamIdFilter = request.TeamId,
                    TournamentIdFilter = request.TournamentId
                });
            if (!participations.Any())
                throw new NotFoundException(
                    $" TeamId: {request.TeamId} TournamentId: {request.TournamentId} StepId :{request.StepId}",
                    "Participation");
            return participations.First();
        }
    }
}