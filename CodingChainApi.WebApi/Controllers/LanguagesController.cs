using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Read.Languages;
using Application.Read.Languages.Handlers;
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


        [HttpGet("{id}", Name = nameof(GetLanguageById))]
        [Produces(typeof(HateoasResponse<ProgrammingLanguageNavigation>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLanguageById(Guid id)
        {
            var language = await Mediator.Send(new GetLanguageNavigationByIdQuery(id));
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

        [HttpPost(TemplateActionName)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Test(TestCommand command)
        {
            using var process = new Process
            {
                StartInfo =
                {
                    FileName = command.InstallationProcess,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    Arguments = command.InstallationCommand
                }
            };
            process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
            process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            return Ok();
        }

        private IList<LinkDto> GetLinksForLanguage(Guid languageId)
        {
            return new List<LinkDto>()
            {
                LinkDto.SelfLink(Url.Link(nameof(GetLanguageById), new {id = languageId}))
            };
        }


        public record TestCommand(string Name, string RunTestsProcess, string RunTestsCommand,
            string InstallationProcess, string InstallationCommand, string TestsFileRelativePath,
            string SutFileRelativePath);

        public record AddEnvironmentToLanguageBodyCommand(
            Guid PlatformId,
            string? TemplateInstallationProcess,
            string? TemplateInstallationCommand,
            string RunTestsProcess,
            string RunTestsCommand,
            string InstallationProcess,
            string InstallationCommand,
            string TestsFileRelativePath,
            string SutFileRelativePath,
            IFormFile Template);
    }
}