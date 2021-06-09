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
using Domain.ParticipationSessions;
using MediatR;

namespace Application.Write.ParticipationsSessions
{
    [Authenticated]
    public record ConnectUserToParticipation(Guid ParticipationId) : IRequest<int>;

    public class ConnectUserToParticipationHandler : IRequestHandler<ConnectUserToParticipation, int>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IParticipationsSessionsRepository _participationsSessionsRepository;
        private readonly IDispatcher<PrepareParticipationExecutionDto> _prepareParticipationExecutionService;
        private readonly IReadParticipationRepository _readParticipationRepository;

        public ConnectUserToParticipationHandler(
            IParticipationsSessionsRepository participationsSessionsRepository, ICurrentUserService currentUserService,
            IDispatcher<PrepareParticipationExecutionDto> prepareParticipationExecutionService,
            IReadParticipationRepository readParticipationRepository)
        {
            _participationsSessionsRepository = participationsSessionsRepository;
            _currentUserService = currentUserService;
            _prepareParticipationExecutionService = prepareParticipationExecutionService;
            _readParticipationRepository = readParticipationRepository;
        }

        public async Task<int> Handle(ConnectUserToParticipation request, CancellationToken cancellationToken)
        {
            var participation =
                await _participationsSessionsRepository.FindByIdAsync(new ParticipationId(request.ParticipationId));
            if (participation is null) throw new NotFoundException(request.ParticipationId.ToString(), "Participation");

            var isFreshParticipation = !participation.HasConnectedUsers;
            var connectionCount = participation.AddConnectedUser(_currentUserService.UserId);
            await _participationsSessionsRepository.SetAsync(participation);
            if (isFreshParticipation) await PrepareParticipation(participation);

            return connectionCount;
        }

        private async Task PrepareParticipation(ParticipationSessionAggregate participation)
        {
            var language =
                await _readParticipationRepository.GetLanguageByParticipation(participation.Id.Value);
            if (language is null)
                throw new NotFoundException(participation.Id.Value.ToString(), "Participation Language");
            _prepareParticipationExecutionService.Dispatch(
                new PrepareParticipationExecutionDto(participation.Id.Value, language.Name));
        }
    }
}