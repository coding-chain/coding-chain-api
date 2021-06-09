using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Write.Contracts;
using MediatR;

namespace Application.Write.Users.EditUser
{
    [Authenticated]
    public record EditUserCommand(string? Username, string? Password, string? Email) : IRequest<string>;

    public class EditUserHandler : IRequestHandler<EditUserCommand, string>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ISecurityService _securityService;
        private readonly IUserRepository _userRepository;

        public EditUserHandler(
            ISecurityService securityService, IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            _securityService = securityService;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            string? hashedPassword = null;
            var (username, password, email) = request;
            if (password is not null) hashedPassword = _securityService.HashPassword(password);

            var user = await _userRepository.FindByIdAsync(_currentUserService.UserId);
            if (user is null) throw new NotFoundException(_currentUserService.UserId.Value.ToString(), "user");

            user.UpdateUser(email, username, hashedPassword);
            await _userRepository.SetAsync(user);
            return user.Id.ToString();
        }
    }
}