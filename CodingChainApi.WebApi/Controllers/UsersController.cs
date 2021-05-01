using System.Threading.Tasks;
using Application.Write.Users.LoginUser;
using Application.Write.Users.RegisterUser;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeosCodingApi.Services;

namespace NeosCodingApi.Controllers
{
    public class UsersController : ApiControllerBase
    {
        public UsersController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) : base(mediator, mapper, propertyCheckerService)
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
            var res = await Mediator.Send(registerUserCommand);
            return CreatedAtRoute(nameof(Authentication), new { });
        }

        [AllowAnonymous]
        [HttpPost(TemplateActionName, Name = nameof(Authentication))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Authentication(LoginUserQuery loginUserQuery)
        {
            var loggedUser = await Mediator.Send(loginUserQuery);
            return Ok(loggedUser);
        }
        
        
    }
}