using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.FunctionSessions.Handlers
{
    public record GetOneParticipationSessionFunctionQuery
        (Guid ParticipationId, Guid FunctionId) : IRequest<FunctionSessionNavigation>;
    public class GetOneParticipationSessionFunctionHandler : IRequestHandler<
        GetOneParticipationSessionFunctionQuery, FunctionSessionNavigation>
    {
        private readonly IReadFunctionSessionRepository _readFunctionSessionRepository;
        private readonly IReadParticipationSessionRepository _readParticipationSessionRepository;

        public GetOneParticipationSessionFunctionHandler(IReadFunctionSessionRepository readFunctionSessionRepository, IReadParticipationSessionRepository readParticipationSessionRepository)
        {
            _readFunctionSessionRepository = readFunctionSessionRepository;
            _readParticipationSessionRepository = readParticipationSessionRepository;
        }

        public async Task<FunctionSessionNavigation> Handle(GetOneParticipationSessionFunctionQuery request,
            CancellationToken cancellationToken)
        {
            if (!await _readParticipationSessionRepository.ExistsById(request.ParticipationId))
            {
                throw new NotFoundException(request.ParticipationId.ToString(), "ParticipationSession");
            }
            var function = await _readFunctionSessionRepository
                .GetOneFunctionNavigationByIdAsync(request.ParticipationId, request.FunctionId);
            if (function is null)
            {
                throw new NotFoundException(
                    $"ParticipationId : {request.ParticipationId}, FunctionId: {request.FunctionId}",
                    "FunctionSession");
            }

            return function;
        }
    }
}