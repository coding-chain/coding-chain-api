using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Read.Rights;
using Application.Read.Users.Handlers;
using Application.Write.Contracts;
using Domain.Users;
using MediatR;
using ApplicationException = Application.Common.Exceptions.ApplicationException;

namespace Application.Write.Users
{
    [Authenticated]
    [Authorize(Roles = new[] { RightEnum.Admin })]
    public record ChangeUserRightsCommand
        (Guid UserId, IList<Guid> RightsIds) : IRequest<string>;

    public class ChangeUserRightsHandler : IRequestHandler<ChangeUserRightsCommand, string>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IReadRightRepository _rightsRepository;
        private readonly IReadUserRepository _readUserRepository;

        public ChangeUserRightsHandler(IUserRepository userRepository, ICurrentUserService currentUserService,
            IReadRightRepository rightsRepository, IReadUserRepository readUserRepository)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _rightsRepository = rightsRepository;
            _readUserRepository = readUserRepository;
        }

        public async Task<string> Handle(ChangeUserRightsCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(new UserId(request.UserId));
            var rights = new List<RightNavigation>();
            foreach (var rightId in request.RightsIds)
            {
                var right = await _rightsRepository.GetOneRightNavigationById(rightId);
                if (right is null)
                    throw new NotFoundException(rightId.ToString(), "Right");
                rights.Add(right);
            }

            if (user is null) throw new NotFoundException(_currentUserService.UserId.Value.ToString(), "user");
            if (user.Rights.Any(r => r.Name == RightEnum.Admin) && rights.All(r => r.Name != RightEnum.Admin))
            {
                var adminRight = await _rightsRepository.GetOneRightNavigationByName(RightEnum.Admin);
                if (adminRight is null)
                    throw new NotFoundException(RightEnum.Admin.ToString(), "Right");

                var adminUsers = await _readUserRepository.FindAllPublicUsers(new GetPublicUsersQuery()
                    { RightIdFilter = adminRight.Id });
                if (adminUsers.Count <= 1)
                    throw new ApplicationException("Cannot remove user admin right if user is the last admin");
            }

            user.SetRights(rights.Select(r => new Right(r.Name)).ToList());
            await _userRepository.SetAsync(user);
            return user.Id.ToString();
        }
    }
}