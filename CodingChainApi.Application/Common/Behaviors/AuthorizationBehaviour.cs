using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Users;
using MediatR;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

namespace Application.Common.Behaviors
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public AuthorizationBehaviour(ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }


        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();
            if (authorizeAttributes.Any())
            {
                // Must be authenticated user
                if (_currentUserService.UserId == null) throw new UnauthorizedAccessException();

                var user = await _userRepository.FindByIdAsync(_currentUserService.UserId);
                var rolesMatch = false;
                var requiredRoleLists = authorizeAttributes
                    .Select(a => a.Roles.Select(r => new Right(r)).ToList());
                foreach (var authorizedRoles in requiredRoleLists)
                {
                    var notMatchingRoles = user.GetNotMatchingRoles(authorizedRoles);
                    if (!notMatchingRoles.Any()) rolesMatch = true;
                }

                if (!rolesMatch) throw new ForbiddenAccessException($"User {user.Id} doesn't has sufficient roles");
            }

            // User is authorized / authorization not required
            return await next();
        }
    }
}