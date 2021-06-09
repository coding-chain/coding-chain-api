using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Read.Participations;
using Application.Read.Participations.Handlers;
using Application.Write.Participations;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeosCodingApi.Helpers;
using NeosCodingApi.Services;
using NSwag.Annotations;

namespace NeosCodingApi.Controllers
{
    public class ParticipationsController : ApiControllerBase
    {
        public ParticipationsController(ISender mediator, IMapper mapper,
            IPropertyCheckerService propertyCheckerService) : base(mediator, mapper, propertyCheckerService)
        {
        }

        #region Links

        private IList<LinkDto> GetLinksForParticipation(Guid participationId)
        {
            return new List<LinkDto>
            {
                LinkDto.CreateLink(Url.Link(nameof(CreateParticipation), null)),
                LinkDto.SelfLink(Url.Link(nameof(GetParticipationById), new {participationId})),
                LinkDto.AllLink(Url.Link(nameof(GetParticipations), null))
            };
        }

        #endregion

        #region Participations

        [HttpPost(Name = nameof(CreateParticipation))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateParticipation(CreateParticipationCommand createParticipationCommand)
        {
            var participationId = await Mediator.Send(createParticipationCommand);
            return CreatedAtAction(nameof(GetParticipationById), new {participationId}, null);
        }


        [HttpGet("{participationId:guid}", Name = nameof(GetParticipationById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<ParticipationNavigation>))]
        public async Task<IActionResult> GetParticipationById(Guid participationId)
        {
            var participation = await Mediator.Send(new GetOneParticipationNavigationByIdQuery(participationId));
            var participationWithLinks =
                new HateoasResponse<ParticipationNavigation>(participation, GetLinksForParticipation(participation.Id));
            return Ok(participationWithLinks);
        }

        [HttpGet(Name = nameof(GetParticipations))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<ParticipationNavigation>>))]
        public async Task<IActionResult> GetParticipations(
            [FromQuery] GetAllParticipationNavigationPaginatedQuery query)
        {
            var participations = await Mediator.Send(query);
            var participationsWithLinks = participations.Select(participation =>
                new HateoasResponse<ParticipationNavigation>(participation,
                    GetLinksForParticipation(participation.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                participations.ToPagedListResume(),
                participationsWithLinks.ToList(),
                nameof(GetParticipations))
            );
        }

        #endregion

        #region Functions

        #endregion
    }
}