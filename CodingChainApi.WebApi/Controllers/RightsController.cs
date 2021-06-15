using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Read.Rights;
using Application.Read.Rights.Handlers;
using AutoMapper;
using CodingChainApi.Helpers;
using CodingChainApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CodingChainApi.Controllers
{
    public class RightsController : ApiControllerBase
    {
        public RightsController(ISender mediator, IMapper mapper,
            IPropertyCheckerService propertyCheckerService) : base(mediator, mapper, propertyCheckerService)
        {
        }

        [HttpGet("{rightId}", Name = nameof(GetRightById))]
        [Produces(typeof(HateoasResponse<RightNavigation>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRightById(Guid rightId)
        {
            var right = await Mediator.Send(new GetRightByIdQuery(rightId));
            var rightWithLinks =
                new HateoasResponse<RightNavigation>(right, GetLinksForRight(right.Id));
            return Ok(rightWithLinks);
        }

        [HttpGet(Name = nameof(GetRights))]
        [SwaggerResponse(HttpStatusCode.OK,
            typeof(HateoasResponse<IList<HateoasResponse<RightNavigation>>>))]
        public async Task<IActionResult> GetRights([FromQuery] GetPaginatedRightNavigationsQuery query)
        {
            var rights = await Mediator.Send(query);
            var rightsWithLinks = rights.Select(right =>
                new HateoasResponse<RightNavigation>(right, GetLinksForRight(right.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                rights.ToPagedListResume(),
                rightsWithLinks.ToList(),
                nameof(GetRights))
            );
        }

        private IList<LinkDto> GetLinksForRight(Guid rightId)
        {
            return new List<LinkDto>
            {
                LinkDto.SelfLink(Url.Link(nameof(GetRightById), new {rightId})),
                LinkDto.AllLink(Url.Link(nameof(GetRights), null))
            };
        }
    }
}