using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Read.Contracts;
using Application.Read.Users.Handlers;
using Application.Write.Contracts;
using Domain.Users;
using MediatR;
using ApplicationException = Application.Common.Exceptions.ApplicationException;

namespace Application.Write.Users
{
    [Authenticated]
    [Authorize(Roles = new[] { RightEnum.Admin })]
    public record DeleteUserCommand(Guid UserId) : IRequest<string>;

    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IReadUserRepository _readUserRepository;
        private readonly IReadRightRepository _readRightRepository;

        public DeleteUserHandler(IUserRepository userRepository, IReadUserRepository readUserRepository,
            IReadRightRepository readRightRepository)
        {
            _userRepository = userRepository;
            _readUserRepository = readUserRepository;
            _readRightRepository = readRightRepository;
        }

        public async Task<string> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(new UserId(request.UserId));
            if (user is null)
                throw new NotFoundException(request.UserId.ToString(), "User");
            if (user.Rights.Any(r => r.Name == RightEnum.Admin))
            {
                var adminRight = await _readRightRepository.GetOneRightNavigationByName(RightEnum.Admin);
                if (adminRight is null)
                    throw new NotFoundException(RightEnum.Admin.ToString(), "Right");

                var adminUsers = await _readUserRepository.FindAllPublicUsers(new GetPublicUsersQuery()
                    { RightIdFilter = adminRight.Id });
                if (adminUsers.Count <= 1)
                    throw new ApplicationException("Cannot delete last existing admin");
            }
            await _userRepository.RemoveAsync(user.Id);
            return user.Id.ToString();
        }
    }
}