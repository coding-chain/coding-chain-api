using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Tournaments.Handlers
{
    public record GetTournamentStepByIdQuery(Guid TournamentId, Guid StepId) : IRequest<TournamentStepNavigation>;

    public class GetTournamentStepByIdHandler : IRequestHandler<GetTournamentStepByIdQuery, TournamentStepNavigation>
    {
        private readonly IReadTournamentRepository _readTournamentRepository;

        public GetTournamentStepByIdHandler(IReadTournamentRepository readTournamentRepository)
        {
            _readTournamentRepository = readTournamentRepository;
        }

        public async Task<TournamentStepNavigation> Handle(GetTournamentStepByIdQuery request,
            CancellationToken cancellationToken)
        {
            var step = await _readTournamentRepository.GetOneTournamentStepNavigationByID(request.TournamentId,
                request.StepId);
            if (step is null)
                throw new NotFoundException($"tournament : {request.TournamentId}, step: {request.StepId}",
                    nameof(TournamentStepNavigation));
            return step;
        }
    }
}