using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Read.Teams;
using Application.Read.Teams.Handlers;
using Application.Read.Tournaments;
using Application.Read.Tournaments.Handlers;
using Application.Write.Tournaments;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeosCodingApi.Helpers;
using NeosCodingApi.Services;
using NSwag.Annotations;

namespace NeosCodingApi.Controllers
{
    public class TournamentsController: ApiControllerBase
    {
        public TournamentsController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) : base(mediator, mapper, propertyCheckerService)
        {
        }
        
        [HttpPost(Name = nameof(CreateTournament))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateTournament(CreateTournamentCommand createTournamentCommand)
        {
            var tournamentId = await Mediator.Send(createTournamentCommand);
            return CreatedAtAction(nameof(GetTournamentById), new {id = tournamentId}, null);
        }
        
        [HttpGet("{id:guid}", Name = nameof(GetTournamentById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<TournamentNavigation>))]
        public async Task<IActionResult> GetTournamentById(Guid id)
        {
            var tournament = await Mediator.Send(new GetTournamentNavigationByIdQuery(id));
            var tournamentWithLinks =  new HateoasResponse<TournamentNavigation>(tournament, GetLinksForTournament(tournament.Id));
            return Ok(tournamentWithLinks);
        }
        
        [HttpGet(Name = nameof(GetTournaments))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<IList<HateoasResponse<TournamentNavigation>>>))]
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
        private IList<LinkDto> GetLinksForTournament(Guid tournamentId)
        {
            return new List<LinkDto>()
            {
                LinkDto.CreateLink(Url.Link(nameof(CreateTournament), null)),
                LinkDto.SelfLink(Url.Link(nameof(GetTournamentById), new {id = tournamentId}))
            };
        }
    }
}