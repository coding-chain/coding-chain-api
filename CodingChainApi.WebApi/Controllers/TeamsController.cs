using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Read.Teams;
using Application.Read.Teams;
using Application.Read.Teams.Handlers;
using Application.Write.Teams;
using AutoMapper;
using CodingChainApi.Infrastructure.Common.Pagination;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeosCodingApi.Helpers;
using NeosCodingApi.Services;
using NSwag.Annotations;

namespace NeosCodingApi.Controllers
{
    public class TeamsController : ApiControllerBase
    {
        public TeamsController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) : base(
            mediator, mapper, propertyCheckerService)
        {
        }

        [HttpPost(Name = nameof(CreateTeam))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateTeam(CreateTeamCommand createTeamCommand)
        {
            var teamId = await Mediator.Send(createTeamCommand);
            return CreatedAtAction(nameof(GetTeamById), new {id = teamId}, null);
        }

        public record AddMemberToTeamBodyCommand(Guid MemberId);

        [HttpPost("{teamId:guid}/members", Name = nameof(AddMemberToTeam))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddMemberToTeam([FromRoute] Guid teamId,
            [FromBody] AddMemberToTeamBodyCommand addMemberToTeamBodyCommand)
        {
            var memberId = await Mediator.Send(new AddMemberToTeamCommand(teamId, addMemberToTeamBodyCommand.MemberId));
            return CreatedAtAction(nameof(GetTeamMemberById), new {teamId, memberId}, null);
        }

        [HttpPost("{teamId:guid}/members/{memberId:guid}/elevation", Name = nameof(ElevateTeamMember))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ElevateTeamMember(Guid teamId, Guid memberId)
        {
            await Mediator.Send(new ElevateTeamMemberCommand(teamId, memberId));
            return NoContent();
        }

        public record RenameTeamCommandBody(string Name);

        [HttpPut("{teamId:guid}", Name = nameof(RenameTeam))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RenameTeam(Guid teamId, [FromBody] RenameTeamCommandBody command)
        {
            await Mediator.Send(new RenameTeamCommand(teamId, command.Name));
            return NoContent();
        }


        [HttpDelete("{teamId:guid}/members/{memberId:guid}", Name = nameof(RemoveMemberFromTeam))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveMemberFromTeam(Guid teamId, Guid memberId)
        {
            await Mediator.Send(new DeleteMemberFromTeamCommand(teamId, memberId));
            return NoContent();
        }


        [HttpDelete("{teamId:guid}", Name = nameof(DeleteTeam))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeam(Guid teamId)
        {
            await Mediator.Send(new DeleteTeamCommand(teamId));
            return NoContent();
        }


        [HttpGet("{teamId:guid}", Name = nameof(GetTeamById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<TeamNavigation>))]
        public async Task<IActionResult> GetTeamById(Guid teamId)
        {
            var team = await Mediator.Send(new GetTeamNavigationByIdQuery(teamId));
            var teamWithLinks = new HateoasResponse<TeamNavigation>(team, GetLinksForTeam(team.Id));
            return Ok(teamWithLinks);
        }


        [HttpGet("{teamId:guid}/members/{memberId:guid}", Name = nameof(GetTeamMemberById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<TeamNavigation>))]
        public async Task<IActionResult> GetTeamMemberById(Guid teamId, Guid memberId)
        {
            var member = await Mediator.Send(new GetMemberNavigationByIdQuery(teamId, memberId));
            var memberWithLinks =
                new HateoasResponse<MemberNavigation>(member, GetLinksForMember(member.UserId, member.TeamId));
            return Ok(memberWithLinks);
        }

        [HttpGet(Name = nameof(GetTeams))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<IList<HateoasResponse<TeamNavigation>>>))]
        public async Task<IActionResult> GetTeams([FromQuery] GetTeamNavigationPaginatedQuery query)
        {
            var teams = await Mediator.Send(query);
            var teamsWithLinks = teams.Select(team =>
                new HateoasResponse<TeamNavigation>(team, GetLinksForTeam(team.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                teams.ToPagedListResume(),
                teamsWithLinks.ToList(),
                nameof(GetTeams))
            );
        }

        private IList<LinkDto> GetLinksForTeam(Guid teamId)
        {
            return new List<LinkDto>()
            {
                LinkDto.CreateLink(Url.Link(nameof(CreateTeam), null)),
                LinkDto.SelfLink(Url.Link(nameof(GetTeamById), new {teamId}))
            };
        }

        private IList<LinkDto> GetLinksForMember(Guid memberId, Guid teamId)
        {
            return new List<LinkDto>()
            {
                LinkDto.CreateLink(Url.Link(nameof(AddMemberToTeam), new {teamId})),
                LinkDto.DeleteLink(Url.Link(nameof(RemoveMemberFromTeam), new {teamId, memberId})),
                LinkDto.SelfLink(Url.Link(nameof(GetTeamMemberById), new {teamId, memberId}))
            };
        }
    }
}