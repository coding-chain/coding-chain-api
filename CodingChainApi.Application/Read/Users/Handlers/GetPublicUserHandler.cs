using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Users.Handlers
{
    public record GetPublicUserQuery : IRequest<PublicUser>;

    public class GetPublicUserHandler : IRequestHandler<GetPublicUserQuery, PublicUser>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IReadUserRepository _readUserRepository;

        public GetPublicUserHandler(ICurrentUserService currentUserService, IReadUserRepository readUserRepository)
        {
            _currentUserService = currentUserService;
            _readUserRepository = readUserRepository;
        }


        public async Task<PublicUser> Handle(GetPublicUserQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is null) throw new ApplicationException("User not connected currently");
            return await _readUserRepository.FindPublicUserById(_currentUserService.UserId.Value) ??
                   throw new ApplicationException($"User not found by its id : {_currentUserService.UserId.Value}");
        }
    }
}