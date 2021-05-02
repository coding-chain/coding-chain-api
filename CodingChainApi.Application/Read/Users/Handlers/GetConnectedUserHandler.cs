using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Users.Handlers
{
    public record GetConnectedUserQuery() : IRequest<ConnectedUser>;

    public class GetConnectedUserHandler : IRequestHandler<GetConnectedUserQuery, ConnectedUser>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IReadUserRepository _readUserRepository;

        public GetConnectedUserHandler(ICurrentUserService currentUserService, IReadUserRepository readUserRepository)
        {
            _currentUserService = currentUserService;
            _readUserRepository = readUserRepository;
        }


        public async Task<ConnectedUser> Handle(GetConnectedUserQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is null)
            {
                throw new ApplicationException("User not connected currently");
            }
            return await _readUserRepository.FindConnectedUserById(_currentUserService.UserId.Value) ??
                   throw new ApplicationException($"User not found by its id : {_currentUserService.UserId.Value}");
        }
    }
}