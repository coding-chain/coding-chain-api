using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Read.Contracts;
using Application.Read.Teams.Handlers;
using MediatR;

namespace Application.Read.Users.Handlers
{ 
    public record GetPublicUserByIdQuery(Guid UserId) : IRequest<PublicUser>;

    public class GetPublicUserByIdHandler: IRequestHandler<GetPublicUserByIdQuery, PublicUser>
    {
        private readonly IReadUserRepository _readUserRepository;

        public GetPublicUserByIdHandler(IReadUserRepository readUserRepository)
        {
            _readUserRepository = readUserRepository;
        }

        public async Task<PublicUser> Handle(GetPublicUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _readUserRepository.FindPublicUserById(request.UserId);
            if (user is null)
                throw new ApplicationException($"User with id {request.UserId} not found");
            return user;
        }
    }
}