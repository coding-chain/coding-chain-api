using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.Dtos;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Participations;
using Domain.ParticipationStates;
using MediatR;

namespace Application.Write.ParticipationsSessions
{
    [Authenticated]
    public record DisconnectUserFromParticipationCommand(Guid ParticipationId) : IRequest<int>;

    public class DisconnectUserFromParticipationHandler : IRequestHandler<DisconnectUserFromParticipationCommand, int>
    {
        private readonly IParticipationsSessionsRepository _participationsSessionsRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IReadParticipationRepository _readParticipationRepository;
        private readonly IDispatcher<CleanParticipationExecutionDto> _cleanParticipationExecutionService;
        public DisconnectUserFromParticipationHandler(
            IParticipationsSessionsRepository participationsSessionsRepository, ICurrentUserService currentUserService, IReadParticipationRepository readParticipationRepository, IDispatcher<CleanParticipationExecutionDto> cleanParticipationExecutionService)
        {
            _participationsSessionsRepository = participationsSessionsRepository;
            _currentUserService = currentUserService;
            _readParticipationRepository = readParticipationRepository;
            _cleanParticipationExecutionService = cleanParticipationExecutionService;
        }

        public async Task<int> Handle(DisconnectUserFromParticipationCommand request, CancellationToken cancellationToken)
        {
            var participation =
                await _participationsSessionsRepository.FindByIdAsync(new ParticipationId(request.ParticipationId));
            if (participation is null)
            {
                throw new NotFoundException(request.ParticipationId.ToString(), "Participation");
            }
            var connectionCount = participation.RemoveConnectedUser(_currentUserService.UserId);
            if (participation.HasConnectedUsers)
            {
                await _participationsSessionsRepository.SetAsync(participation);
            }
            else
            {
                await _participationsSessionsRepository.RemoveAsync(participation.Id);
                await CleanParticipation(participation);
            }
            return connectionCount;
        }

        private async Task CleanParticipation(ParticipationSessionAggregate participation)
        {
            var language =
                await _readParticipationRepository.GetLanguageByParticipation(participation.Id.Value);
            if (language is null)
                throw new NotFoundException(participation.Id.Value.ToString(), "Participation Language");
            _cleanParticipationExecutionService.Dispatch(
                new CleanParticipationExecutionDto(participation.Id.Value, language.Name));
        }
    }
}