using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Read.ProgrammingLanguages;
using Application.Read.ProgrammingLanguages.Handlers;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeosCodingApi.Helpers;
using NeosCodingApi.Services;
using NSwag.Annotations;

namespace NeosCodingApi.Controllers
{
    public class LanguagesController : ApiControllerBase
    {
        public LanguagesController(IMediator mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) :
            base(mediator, mapper, propertyCheckerService)
        {
        }


        [HttpGet("{languageId}", Name = nameof(GetLanguageById))]
        [Produces(typeof(HateoasResponse<ProgrammingLanguageNavigation>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLanguageById(Guid languageId)
        {
            var language = await Mediator.Send(new GetLanguageNavigationByIdQuery(languageId));
            var languageWithLinks =
                new HateoasResponse<ProgrammingLanguageNavigation>(language, GetLinksForLanguage(language.Id));
            return Ok(languageWithLinks);
        }

        [HttpGet(Name = nameof(GetLanguages))]
        [SwaggerResponse(HttpStatusCode.OK,
            typeof(HateoasResponse<IList<HateoasResponse<ProgrammingLanguageNavigation>>>))]
        public async Task<IActionResult> GetLanguages([FromQuery] GetAllProgrammingLanguagesPaginatedQuery query)
        {
            var languages = await Mediator.Send(query);
            var languagesWithLinks = languages.Select(language =>
                new HateoasResponse<ProgrammingLanguageNavigation>(language, GetLinksForLanguage(language.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                languages.ToPagedListResume(),
                languagesWithLinks.ToList(),
                nameof(GetLanguages))
            );
        }

        private IList<LinkDto> GetLinksForLanguage(Guid languageId)
        {
            return new List<LinkDto>()
            {
                LinkDto.SelfLink(Url.Link(nameof(GetLanguageById), new {languageId})),
                LinkDto.AllLink(Url.Link(nameof(GetLanguages), null))
            };
        }


      

       
    }
}