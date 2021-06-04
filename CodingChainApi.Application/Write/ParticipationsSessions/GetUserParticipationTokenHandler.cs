using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Users.LoginUser;
using MediatR;

namespace Application.Write.ParticipationsSessions
{
        
    [Authenticated]
    public record GetUserParticipationTokenQuery(Guid ParticipationId) : IRequest<TokenResponse>;
    public class GetUserParticipationTokenHandler : IRequestHandler<GetUserParticipationTokenQuery, TokenResponse>
    {
        private readonly IReadParticipationRepository _readParticipationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITokenService _tokenService;

        public GetUserParticipationTokenHandler(IReadParticipationRepository readParticipationRepository, ICurrentUserService currentUserService, ITokenService tokenService)
        {
            _readParticipationRepository = readParticipationRepository;
            _currentUserService = currentUserService;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> Handle(GetUserParticipationTokenQuery request, CancellationToken cancellationToken)
        {
            if (!await _readParticipationRepository.ExistsById(request.ParticipationId))
            {
                throw new NotFoundException(request.ParticipationId.ToString(), "Participation");
            }

            var token = await _tokenService.GenerateUserParticipationTokenAsync(
                _currentUserService.UserId.Value, request.ParticipationId);
            return new TokenResponse(token);
        }
    }
}