using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Teams.Handlers
{
    public record GetMemberNavigationByIdQuery(Guid TeamId, Guid MemberId) : IRequest<MemberNavigation>;

    public class GetMemberNavigationById: IRequestHandler<GetMemberNavigationByIdQuery, MemberNavigation>
    {
        private IReadTeamRepository _readTeamRepository;

        public GetMemberNavigationById(IReadTeamRepository readTeamRepository)
        {
            _readTeamRepository = readTeamRepository;
        }

        public async Task<MemberNavigation> Handle(GetMemberNavigationByIdQuery request, CancellationToken cancellationToken)
        {
            return  await _readTeamRepository.GetOneMemberNavigationByIdAsync(request.TeamId, request.MemberId) ?? 
                    throw new NotFoundException($"team: {request.TeamId}, member:  {request.MemberId}", nameof(MemberNavigation));
        }
    }
}