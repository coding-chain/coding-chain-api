using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Read.Users;
using Application.Read.Users.Handlers;
using Application.Write.Users.EditUser;
using Application.Write.Users.LoginUser;
using Application.Write.Users.RegisterUser;
using AutoMapper;
using CodingChainApi.Helpers;
using CodingChainApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CodingChainApi.Controllers
{
    public class UsersController : ApiControllerBase
    {
        public UsersController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) : base(
            mediator, mapper, propertyCheckerService)
        {
        }

        [AllowAnonymous]
        [HttpPost(TemplateActionName, Name = nameof(Registration))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registration(RegisterUserCommand registerUserCommand)
        {
            try
            {
                var res = await Mediator.Send(registerUserCommand);
                return CreatedAtRoute(nameof(Authentication), new { });
            }
            catch (ApplicationException e)
            {
                return Conflict();
            }
        }

        [AllowAnonymous]
        [HttpPost(TemplateActionName, Name = nameof(Authentication))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Authentication(LoginUserQuery loginUserQuery)
        {
            try
            {
                var userToken = await Mediator.Send(loginUserQuery);
                return Ok(userToken);
            }
            catch (ApplicationException e)
            {
                return NotFound();
            }
        }

        [HttpGet(TemplateActionName, Name = nameof(Me))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(PublicUser))]
        public async Task<IActionResult> Me()
        {
            try
            {
                var loggedUser = await Mediator.Send(new GetPublicUserQuery());
                return Ok(loggedUser);
            }
            catch (ApplicationException e)
            {
                return Unauthorized();
            }
        }

        [HttpPut(TemplateActionName, Name = nameof(Me))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Me(EditUserCommand cmd)
        {
            try
            {
                await Mediator.Send(cmd);
                return NoContent();
            }
            catch (ApplicationException e)
            {
                return Unauthorized();
            }
        }

        [HttpGet(Name = nameof(GetAllUsers))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<PublicUser>>))]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetPaginatedPublicUsersQuery query)
        {
            var users = await Mediator.Send(query);
            var usersWithLinks = users.Select(user =>
                new HateoasResponse<PublicUser>(user, GetLinksForUser(user.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                users.ToPagedListResume(),
                usersWithLinks.ToList(),
                nameof(GetAllUsers)));
        }

        [HttpGet("{userId:guid}", Name = nameof(GetUserById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<PublicUser>))]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await Mediator.Send(new GetPublicUserByIdQuery(userId));
            var userWithLinks = new HateoasResponse<PublicUser>(user, GetLinksForUser(user.Id));
            return Ok(userWithLinks);
        }

        private IList<LinkDto> GetLinksForUser(Guid userId)
        {
            return new List<LinkDto>
            {
                LinkDto.SelfLink(Url.Link(nameof(GetUserById), new {userId}))
            };
        }
    }
}