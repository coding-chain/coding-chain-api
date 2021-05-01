using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Users;
using MediatR;

namespace Application.Write.Users.LoginUser
{
    public record LoginUserQuery(string Password, string Email) : IRequest<UserTokenResponse>;

    public record UserTokenResponse(string Token);

    public class LoginUserHandler : IRequestHandler<LoginUserQuery, UserTokenResponse>
    {
        private readonly ISecurityService _securityService;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IReadUserRepository _readUserRepository;

        public LoginUserHandler(ISecurityService securityService, ITokenService tokenService,
            IUserRepository userRepository, IReadUserRepository readUserRepository)
        {
            _securityService = securityService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _readUserRepository = readUserRepository;
        }

        public async Task<UserTokenResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var userId = await _readUserRepository.FindUserIdByEmail(request.Email);
            if (userId is null) throw new ApplicationException($"User {request.Email} not found");
            var user = await _userRepository.FindByIdAsync(new UserId(userId.Value));
            if (user is null) throw new ApplicationException($"User {request.Email} not found");
            
            
            if (!_securityService.ValidatePassword(request.Password, user.Password))
                throw new ApplicationException("Invalid credentials");

            var token = await _tokenService.GenerateUserTokenAsync(user);

            return new UserTokenResponse(token);
        }
    }
}