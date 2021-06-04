using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Application.Write.Users.LoginUser;
using Domain.Participations;
using MediatR;

namespace Application.Write.ParticipationsSessions
{
    [Authenticated]
    public record ConnectUserToParticipation(Guid ParticipationId) : IRequest<int>;

    public class ConnectUserToParticipationHandler : IRequestHandler<ConnectUserToParticipation, int>
    {
        private readonly IParticipationsSessionsRepository _participationsSessionsRepository;
        private readonly ICurrentUserService _currentUserService;
        public ConnectUserToParticipationHandler(IParticipationRepository participationRepository,
            IParticipationsSessionsRepository participationsSessionsRepository, ICurrentUserService currentUserService)
        {
            _participationsSessionsRepository = participationsSessionsRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(ConnectUserToParticipation request, CancellationToken cancellationToken)
        {
            var participation =
                await _participationsSessionsRepository.FindByIdAsync(new ParticipationId(request.ParticipationId));
            if (participation is null)
            {
                throw new NotFoundException(request.ParticipationId.ToString(), "Participation");
            }
            var connectionCount = participation.AddConnectedUser(_currentUserService.UserId);
            await _participationsSessionsRepository.SetAsync(participation);
            return connectionCount;
        }
    }

}