using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Participations;
using Application.Read.Participations.Handlers;
using Application.Read.Teams;
using Application.Read.Teams.Handlers;
using Application.Read.Tournaments;
using Application.Read.Tournaments.Handlers;
using Application.Write.Tournaments;
using AutoMapper;
using CodingChainApi.Helpers;
using CodingChainApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CodingChainApi.Controllers
{
    public class TournamentsController : ApiControllerBase
    {
        public TournamentsController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) :
            base(mediator, mapper, propertyCheckerService)
        {
        }

        #region Tournaments

        [HttpPost(Name = nameof(CreateTournament))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateTournament(CreateTournamentCommand createTournamentCommand)
        {
            var tournamentId = await Mediator.Send(createTournamentCommand);
            return CreatedAtAction(nameof(GetTournamentById), new {tournamentId}, null);
        }

        [HttpGet("{tournamentId:guid}", Name = nameof(GetTournamentById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<TournamentNavigation>))]
        public async Task<IActionResult> GetTournamentById(Guid tournamentId)
        {
            var tournament = await Mediator.Send(new GetTournamentNavigationByIdQuery(tournamentId));
            var tournamentWithLinks =
                new HateoasResponse<TournamentNavigation>(tournament, GetLinksForTournament(tournament.Id));
            return Ok(tournamentWithLinks);
        }

        [HttpDelete("{tournamentId:guid}", Name = nameof(DeleteTournament))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTournament(Guid tournamentId)
        {
            await Mediator.Send(new DeleteTournamentCommand(tournamentId));
            return NoContent();
        }

        [HttpGet(Name = nameof(GetTournaments))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<TournamentNavigation>>))]
        public async Task<IActionResult> GetTournaments([FromQuery] GetTournamentNavigationPaginatedQuery query)
        {
            var tournaments = await Mediator.Send(query);
            var tournamentsWithLinks = tournaments.Select(tournament =>
                new HateoasResponse<TournamentNavigation>(tournament, GetLinksForTournament(tournament.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                tournaments.ToPagedListResume(),
                tournamentsWithLinks.ToList(),
                nameof(GetTournaments))
            );
        }

        public record UpdateTournamentCommandBody(string Name, string Description, bool IsPublished,
            DateTime? StartDate,
            DateTime? EndDate);

        [HttpPut("{tournamentId:guid}", Name = nameof(UpdateTournament))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateTournament(Guid tournamentId,
            [FromBody] UpdateTournamentCommandBody command)
        {
            await Mediator.Send(new UpdateTournamentCommand(
                tournamentId,
                command.Name,
                command.Description,
                command.IsPublished,
                command.StartDate,
                command.EndDate
            ));
            return NoContent();
        }

        public record SetTournamentStepsCommandBody(IList<TournamentStep> Steps);

        [HttpPut("{tournamentId:guid}/steps", Name = nameof(UpdateTournamentSteps))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateTournamentSteps(Guid tournamentId,
            [FromBody] SetTournamentStepsCommandBody command)
        {
            await Mediator.Send(new SetTournamentStepsCommand(tournamentId, command.Steps));
            return NoContent();
        }

        #endregion

        #region Steps

        [HttpDelete("{tournamentId:guid}/steps/{stepId:guid}", Name = nameof(DeleteTournamentStep))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTournamentStep(Guid tournamentId, Guid stepId)
        {
            await Mediator.Send(new DeleteTournamentStepCommand(tournamentId, stepId));
            return NoContent();
        }


        [HttpGet("{tournamentId:guid}/steps/{stepId:guid}", Name = nameof(GetTournamentStepById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<TournamentStepNavigation>))]
        public async Task<IActionResult> GetTournamentStepById(Guid tournamentId, Guid stepId)
        {
            var member = await Mediator.Send(new GetTournamentStepByIdQuery(tournamentId, stepId));
            var memberWithLinks =
                new HateoasResponse<TournamentStepNavigation>(member, GetLinksForTournamentStep(tournamentId, stepId));
            return Ok(memberWithLinks);
        }

        public record GetTournamentNavigationPaginatedQueryParameters : PaginationQueryBase;

        [HttpGet("{tournamentId:guid}/steps", Name = nameof(GetTournamentSteps))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<TournamentStepNavigation>>))]
        public async Task<IActionResult> GetTournamentSteps(Guid tournamentId,
            [FromQuery] GetTournamentNavigationPaginatedQueryParameters query)
        {
            var steps = await Mediator.Send(new GetPaginatedTournamentStepNavigationQuery
            {
                Page = query.Page,
                Size = query.Size,
                TournamentId = tournamentId
            });
            var stepsWithLinks = steps.Select(step =>
                new HateoasResponse<TournamentStepNavigation>(step,
                    GetLinksForTournamentStep(tournamentId, step.StepId)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                steps.ToPagedListResume(),
                stepsWithLinks.ToList(),
                nameof(GetTournamentSteps))
            );
        }
        
        public record GetLeaderBoardTeamsPaginatedQueryParams: PaginationQueryBase
        {
            public OrderEnum? ScoreOrder { get; set; }
            public bool? HasFinishedFilter { get; set; }
        }
        [HttpGet("{tournamentId:guid}/teams",Name = nameof(GetLeaderBoardTeams))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<LeaderBoardTeamNavigation>>))]
        public async Task<IActionResult> GetLeaderBoardTeams(Guid tournamentId, [FromQuery] GetLeaderBoardTeamsPaginatedQueryParams query)
        {
            var teams = await Mediator.Send(new GetLeaderBoardTeamsPaginatedQuery()
            {
                HasFinishedFilter = query.HasFinishedFilter,
                ScoreOrder = query.ScoreOrder,
                Page = query.Page,
                Size = query.Size,
                TournamentIdFilter = tournamentId
            });
            var teamsWithLinks = teams.Select(team =>
                new HateoasResponse<LeaderBoardTeamNavigation>(team, GetLinksForTeam(team.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                teams.ToPagedListResume(),
                teamsWithLinks.ToList(),
                nameof(GetLeaderBoardTeams))
            );
        }

        private IList<LinkDto> GetLinksForTeam(Guid teamId)
        {
            return new List<LinkDto>
            {
                LinkDto.CreateLink(Url.Link(nameof(TeamsController.CreateTeam), null)),
                LinkDto.SelfLink(Url.Link(nameof(TeamsController.GetTeamById), new {teamId}))
            };
        }

        [HttpGet("{tournamentId:guid}/participations", Name = nameof(GetTournamentParticipations))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<ParticipationNavigation>>))]
        public async Task<IActionResult> GetTournamentParticipations(Guid tournamentId,
            [FromQuery] GetTournamentNavigationPaginatedQueryParameters query)
        {
            var participations = await Mediator.Send(new GetAllParticipationNavigationPaginatedQuery
            {
                Page = query.Page,
                Size = query.Size,
                TournamentIdFilter = tournamentId
            });
            var participationsWithLinks = participations.Select(participation =>
                new HateoasResponse<ParticipationNavigation>(participation,
                    GetLinksForTournamentParticipation(tournamentId)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                participations.ToPagedListResume(),
                participationsWithLinks.ToList(),
                nameof(GetTournamentParticipations))
            );
        }

        #endregion

        #region Links

        private IList<LinkDto> GetLinksForTournament(Guid tournamentId)
        {
            return new List<LinkDto>
            {
                LinkDto.CreateLink(Url.Link(nameof(CreateTournament), null)),
                LinkDto.SelfLink(Url.Link(nameof(GetTournamentById), new {tournamentId})),
                LinkDto.DeleteLink(Url.Link(nameof(DeleteTournament), new {tournamentId})),
                LinkDto.UpdateLink(Url.Link(nameof(UpdateTournament), new {tournamentId})),
                LinkDto.UpdateLink(Url.Link(nameof(UpdateTournamentSteps), new {tournamentId}))
            };
        }

        private IList<LinkDto> GetLinksForTournamentStep(Guid tournamentId, Guid stepId)
        {
            return new List<LinkDto>
            {
                LinkDto.DeleteLink(Url.Link(nameof(DeleteTournamentStep), new {tournamentId, stepId})),
                LinkDto.SelfLink(Url.Link(nameof(GetTournamentStepById), new {tournamentId, stepId})),
                LinkDto.AllLink(Url.Link(nameof(GetTournamentSteps), new {tournamentId}))
            };
        }

        private IList<LinkDto> GetLinksForTournamentParticipation(Guid tournamentId)
        {
            return new List<LinkDto>
            {
                LinkDto.AllLink(Url.Link(nameof(GetTournamentParticipations), new {tournamentId}))
            };
        }

        #endregion
    }
}