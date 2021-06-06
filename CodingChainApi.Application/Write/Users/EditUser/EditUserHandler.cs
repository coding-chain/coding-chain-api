using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Users;
using MediatR;

namespace Application.Write.Users.EditUser
{
    [Authenticated]
    public record EditUserCommand(String Username, String Password, String Email) : IRequest<string>;

    public class EditUserHandler : IRequestHandler<EditUserCommand, string>
    {
        private readonly ISecurityService _securityService;
        private readonly IUserRepository _userRepository;
        private readonly IReadUserRepository _readUserRepository;
        private readonly ICurrentUserService _currentUserService;

        public EditUserHandler(
            ISecurityService securityService, IUserRepository userRepository, IReadUserRepository readUserRepository, ICurrentUserService currentUserService)
        {
            _securityService = securityService;
            _userRepository = userRepository;
            _readUserRepository = readUserRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            String password = null;
            if (request.Password != null)
            {
                password = _securityService.HashPassword(request.Password);
            }
            var user = await _userRepository.FindByIdAsync(_currentUserService.UserId);
            if (user is null)
            {
                throw new NotFoundException(_currentUserService.UserId.Value.ToString() , "user");
            }
            user.UpdateUser(request.Email, request.Username, password);

            await _userRepository.SetAsync(user);
            return user.Id.ToString();
        }
    }
}