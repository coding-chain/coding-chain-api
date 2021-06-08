using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.FunctionSessions;
using Application.Read.FunctionSessions.Handlers;
using Application.Read.ParticipationSessions;
using Application.Read.ParticipationSessions.Handlers;
using Application.Read.UserSessions;
using Application.Read.UserSessions.Handlers;
using Application.Write.ParticipationsSessions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using NeosCodingApi.Helpers;
using NeosCodingApi.Services;
using NSwag.Annotations;

namespace NeosCodingApi.Controllers
{
    public class ParticipationSessionsController : ApiControllerBase
    {
        public ParticipationSessionsController(ISender mediator, IMapper mapper,
            IPropertyCheckerService propertyCheckerService) : base(mediator, mapper, propertyCheckerService)
        {
        }

        [HttpPost("{participationId:guid}/execution", Name = nameof(RunExecutionParticipation))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RunExecutionParticipation(Guid participationId)
        {
            var res = await Mediator.Send(new RunParticipationCommand(participationId));
            return NoContent();
        }

        [HttpGet("{participationId:guid}/authentication", Name = nameof(AuthenticateUserOnParticipation))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AuthenticateUserOnParticipation(Guid participationId)
        {
            var token = await Mediator.Send(new GetUserParticipationTokenQuery(participationId));
            return Ok(token);
        }

        [HttpPost("{participationId:guid}/functions", Name = nameof(AddSessionFunction))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddSessionFunction(Guid participationId,
            AddFunctionParticipationCommandBody body)
        {
            var functionId =
                await Mediator.Send(new AddFunctionParticipationSessionCommand(participationId, body.Code, body.Order));
            return CreatedAtAction(nameof(GetSessionFunctionById), new {participationId, functionId}, null);
        }

        [HttpGet("{participationId:guid}", Name = nameof(GetParticipationSessionById))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<ParticipationSessionNavigation>))]
        public async Task<IActionResult> GetParticipationSessionById(Guid participationId)
        {
            var participation = await Mediator.Send(new GetOneParticipationSessionNavigationByIdQuery(participationId));
            var participationWithLinks =
                new HateoasResponse<ParticipationSessionNavigation>(participation,
                    GetLinksForParticipation(participation.Id));
            return Ok(participationWithLinks);
        }


        [HttpDelete("{participationId:guid}/functions/{functionId:guid}", Name = nameof(RemoveSessionFunction))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveSessionFunction(Guid participationId, Guid functionId)
        {
            await Mediator.Send(new RemoveFunctionParticipationSessionCommand(participationId, functionId));
            return NoContent();
        }


        [HttpPost("{participationId:guid}/users/{userId:guid}/elevation", Name = nameof(ElevateMember))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ElevateMember(Guid participationId, Guid userId)
        {
            await Mediator.Send(
                new ElevateSessionUserCommand(participationId, userId));
            return NoContent();
        }

        [HttpPut("{participationId:guid}/functions/{functionId:guid}", Name = nameof(UpdateSessionFunction))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateSessionFunction(Guid participationId, Guid functionId,
            UpdateFunctionParticipationCommandBody body)
        {
            await Mediator.Send(
                new UpdateParticipationSessionFunctionCommand(participationId, functionId, body.Code, body.Order));
            return NoContent();
        }

        [HttpGet("{participationId:guid}/functions/{functionId:guid}", Name = nameof(GetSessionFunctionById))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<FunctionSessionNavigation>))]
        public async Task<IActionResult> GetSessionFunctionById(Guid participationId, Guid functionId)
        {
            var function =
                await Mediator.Send(new GetOneParticipationSessionFunctionQuery(participationId, functionId));
            var functionWithLinks =
                new HateoasResponse<FunctionSessionNavigation>(function,
                    GetLinksForFunction(participationId, functionId));
            return Ok(functionWithLinks);
        }

        [HttpGet("{participationId:guid}/functions", Name = nameof(GetFunctions))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<FunctionSessionNavigation>>))]
        public async Task<IActionResult> GetFunctions(Guid participationId,
            [FromQuery] GetParticipationSessionFunctionsPaginatedQueryParameters query)
        {
            var functions = await Mediator.Send(new GetParticipationSessionFunctionsPaginatedQuery
            {
                ParticipationId = participationId, Page = query.Page, Size = query.Size,
                FunctionsIdsFilter = query.FunctionsIdsFilter
            });
            var functionsWithLinks = functions.Select(function =>
                new HateoasResponse<FunctionSessionNavigation>(function,
                    GetLinksForFunction(participationId, function.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                functions.ToPagedListResume(),
                functionsWithLinks.ToList(),
                nameof(GetFunctions))
            );
        }


        [HttpGet("{participationId:guid}/users", Name = nameof(GetUsers))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<UserSessionNavigation>>))]
        public async Task<IActionResult> GetUsers(Guid participationId, [FromQuery] PaginationQueryBase query)
        {
            var users = await Mediator.Send(new GetParticipationSessionUsersPaginatedQuery
                {ParticipationId = participationId, Page = query.Page, Size = query.Size});
            var usersWithLinks = users.Select(user =>
                new HateoasResponse<UserSessionNavigation>(user,
                    GetLinksForUser(participationId, user.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                users.ToPagedListResume(),
                usersWithLinks.ToList(),
                nameof(GetUsers))
            );
        }


        [HttpGet("{participationId:guid}/users/{userId:guid}", Name = nameof(GetSessionUserById))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<UserSessionNavigation>))]
        public async Task<IActionResult> GetSessionUserById(Guid participationId, Guid userId)
        {
            var user =
                await Mediator.Send(new GetOneParticipationSessionUserQuery(participationId, userId));
            var userWithLinks =
                new HateoasResponse<UserSessionNavigation>(user,
                    GetLinksForUser(participationId, userId));
            return Ok(userWithLinks);
        }

        private IList<LinkDto> GetLinksForFunction(Guid participationId, Guid functionId)
        {
            return new List<LinkDto>
            {
                LinkDto.CreateLink(Url.Link(nameof(AddSessionFunction), new {participationId})),
                LinkDto.DeleteLink(Url.Link(nameof(RemoveSessionFunction), new {participationId, functionId})),
                LinkDto.AllLink(Url.Link(nameof(GetFunctions), new {participationId})),
                LinkDto.SelfLink(Url.Link(nameof(GetSessionFunctionById), new {participationId, functionId})),
                LinkDto.UpdateLink(Url.Link(nameof(UpdateSessionFunction), new {participationId, functionId}))
            };
        }

        private IList<LinkDto> GetLinksForUser(Guid participationId, Guid userId)
        {
            return new List<LinkDto>
            {
                LinkDto.AllLink(Url.Link(nameof(GetUsers), new {participationId})),
                LinkDto.SelfLink(Url.Link(nameof(GetSessionUserById), new {participationId, userId})),
                LinkDto.CreateLink(Url.Link(nameof(ElevateMember), new {participationId, userId}))
            };
        }


        private IList<LinkDto> GetLinksForParticipation(Guid participationId)
        {
            return new List<LinkDto>
            {
                LinkDto.SelfLink(Url.Link(nameof(GetParticipationSessionById), new {participationId})),
                LinkDto.AuthLink(Url.Link(nameof(AuthenticateUserOnParticipation), new {participationId})),
                new(Url.Link(nameof(RunExecutionParticipation), new {participationId}), "execution", HttpMethod.Post)
            };
        }

        public record AddFunctionParticipationCommandBody(string Code,
            int? Order);


        public record UpdateFunctionParticipationCommandBody(string Code,
            int? Order);

        public record GetParticipationSessionFunctionsPaginatedQueryParameters : PaginationQueryBase
        {
            public IList<Guid>? FunctionsIdsFilter { get; set; }
        }
    }
}