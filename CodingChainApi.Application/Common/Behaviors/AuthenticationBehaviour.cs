using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Contracts.IService;
using Application.Write.Contracts;
using Domain.Users;
using MediatR;

namespace Application.Common.Behaviors
{
    public class AuthenticationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public AuthenticationBehaviour(ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }


        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthenticatedAttribute>().ToList().FirstOrDefault();
            if (authorizeAttributes is null)
            {
                return await next();
            }

            if (_currentUserService.UserId is null)  throw new UnauthorizedAccessException("User has to be authenticated");

            var user = await _userRepository.FindByIdAsync(_currentUserService.UserId);
            if(user is null) throw new UnauthorizedAccessException($"User with id {_currentUserService.UserId.Value} not found for authentication");
            return await next();
        }
    }
}