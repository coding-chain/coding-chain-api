using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Read.Languages;
using Application.Read.Languages.Handlers;
using Application.Read.ProgrammingLanguages.Handlers;
using Application.Write.Languages;
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
            // var test = mediator.Publish(new TestEvent(new LanguageId(Guid.NewGuid()))
            //     .ToNotification());
        }


        // [HttpPost]
        // [ProducesResponseType(StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status409Conflict)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        // [Consumes("multipart/form-data")]
        // public async Task<IActionResult> CreateLanguageWithEnvironment(
        //     [ModelBinder(typeof(JsonWithFilesFormDataModelBinder), Name = "json")]
        //     CreateLanguageWithEnvironmentCommand createLanguageWithEnvironmentCommand)
        // {
        //     var languageId = await Mediator.Send(new Application.Write.Languages.CreateLanguageWithEnvironmentCommand(
        //         createLanguageWithEnvironmentCommand.Name,
        //         createLanguageWithEnvironmentCommand.PlatformId,
        //         createLanguageWithEnvironmentCommand.TemplateInstallationProcess,
        //         createLanguageWithEnvironmentCommand.TemplateInstallationCommand,
        //         createLanguageWithEnvironmentCommand.RunTestsProcess,
        //         createLanguageWithEnvironmentCommand.RunTestsCommand,
        //         createLanguageWithEnvironmentCommand.InstallationProcess,
        //         createLanguageWithEnvironmentCommand.InstallationCommand,
        //         createLanguageWithEnvironmentCommand.TestsFileRelativePath,
        //         createLanguageWithEnvironmentCommand.SutFileRelativePath,
        //         createLanguageWithEnvironmentCommand.Template.OpenReadStream()
        //     ));
        //     return Ok();
        // }
        //
        [HttpPost(Name = nameof(CreateLanguage))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateLanguage(
            CreateLanguageCommand createLanguageCommand)
        {
            var languageId = await Mediator.Send(createLanguageCommand);
            return CreatedAtAction(nameof(GetLanguageById), new {id = languageId}, null);
        }


        [HttpGet("{id}", Name = nameof(GetLanguageById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLanguageById(Guid id)
        {
            var language = await Mediator.Send(new GetLanguageNavigationByIdQuery(id));
            return Ok(language);
        }

        [HttpGet(Name = nameof(GetLanguages))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<IList<HateoasResponse<ProgrammingLanguageNavigation>>>))]
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
                LinkDto.CreateLink(Url.Link(nameof(CreateLanguage), null)),
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
