using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Users;
using MediatR;

namespace Application.Write.Users.RegisterUser
{
    public record RegisterUserCommand(string Username, string Password, string Email) : IRequest<string>;

    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly ISecurityService _securityService;
        private readonly IUserRepository _userRepository;
        private readonly IReadUserRepository _readUserRepository;

        public RegisterUserHandler(
            ISecurityService securityService, IUserRepository userRepository, IReadUserRepository readUserRepository)
        {
            _securityService = securityService;
            _userRepository = userRepository;
            _readUserRepository = readUserRepository;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var password = _securityService.HashPassword(request.Password);
            var userExists = await _readUserRepository.UserExistsByEmailAsync(request.Email);
            var users = await _userRepository.GetAllAsync();
            var id = await _userRepository.NextIdAsync();
            if (userExists ) throw new ApplicationException("Username or email already exists");
            var user = new UserAggregate(id, password, new List<Right>(), request.Email,
                request.Username);

            user.SetMandatoryRoles(users);

            await _userRepository.SetAsync(user);
            return id.ToString();
        }
    }
}