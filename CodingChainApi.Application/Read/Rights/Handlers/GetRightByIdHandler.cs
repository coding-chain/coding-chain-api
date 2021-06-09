using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Rights.Handlers
{
    public record GetRightByIdQuery(Guid RightId) : IRequest<RightNavigation>;

    public class GetRightByIdHandler : IRequestHandler<GetRightByIdQuery, RightNavigation>
    {
        private readonly IReadRightRepository _readRightRepository;

        public GetRightByIdHandler(IReadRightRepository readRightRepository)
        {
            _readRightRepository = readRightRepository;
        }

        public async Task<RightNavigation> Handle(GetRightByIdQuery request, CancellationToken cancellationToken)
        {
            var right = await _readRightRepository.GetOneRightNavigationById(request.RightId);
            if (right is null)
                throw new NotFoundException(request.RightId.ToString(), "Right");
            return right;
        }
    }
}