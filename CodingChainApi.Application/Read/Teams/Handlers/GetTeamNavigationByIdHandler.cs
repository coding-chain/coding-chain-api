using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Read.Contracts;
using MediatR;
using ApplicationException = Application.Common.Exceptions.ApplicationException;

namespace Application.Read.Teams.Handlers
{
    public record GetTeamNavigationByIdQuery(Guid TeamId) : IRequest<TeamNavigation>;

    public class GetTeamNavigationByIdHandler : IRequestHandler<GetTeamNavigationByIdQuery, TeamNavigation>

    {
        private readonly IReadTeamRepository _readTeamRepository;

        public GetTeamNavigationByIdHandler(IReadTeamRepository readTeamRepository)
        {
            _readTeamRepository = readTeamRepository;
        }

        public async Task<TeamNavigation> Handle(GetTeamNavigationByIdQuery request,
            CancellationToken cancellationToken)
        {
            var team = await _readTeamRepository.GetOneTeamNavigationByIdAsync(request.TeamId);
            if (team is null)
                throw new ApplicationException($"Team with id {request.TeamId} not found");
            return team;
        }
    }
}