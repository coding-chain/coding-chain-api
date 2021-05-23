using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Steps;
using Application.Read.Steps.Handlers;
using Application.Read.Tests;
using Application.Read.Tests.Handlers;
using Application.Write.StepEditions;
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
    public class StepsController : ApiControllerBase

    {
        public StepsController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService)
            : base(mediator, mapper, propertyCheckerService)
        {
        }

        #region Steps

        [HttpPost(Name = nameof(CreateStep))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateStep(CreateStepCommand createStepCommand)
        {
            var stepId = await Mediator.Send(createStepCommand);
            return CreatedAtAction(nameof(GetStepById), new {stepId}, null);
        }

        [HttpGet("{stepId:guid}", Name = nameof(GetStepById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<StepNavigation>))]
        public async Task<IActionResult> GetStepById(Guid stepId)
        {
            var step = await Mediator.Send(new GetStepNavigationByIdQuery(stepId));
            var stepWithLinks = new HateoasResponse<StepNavigation>(step, GetLinksForStep(step.Id));
            return Ok(stepWithLinks);
        }

        [HttpGet(Name = nameof(GetSteps))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<StepNavigation>>))]
        public async Task<IActionResult> GetSteps([FromQuery] GetPaginatedStepNavigationQuery query)
        {
            var steps = await Mediator.Send(query);
            var stepsWithLinks = steps.Select(step =>
                new HateoasResponse<StepNavigation>(step, GetLinksForStep(step.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                steps.ToPagedListResume(),
                stepsWithLinks.ToList(),
                nameof(GetSteps))
            );
        }

        public record GetPaginatedTestNavigationQueryParams() : PaginationQueryBase;

        [HttpGet("{stepId:guid}/tests", Name = nameof(GetStepTests))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<TestNavigation>>))]
        public async Task<IActionResult> GetStepTests(Guid stepId,
            [FromQuery] GetPaginatedTestNavigationQueryParams query)
        {
            var tests = await Mediator.Send(new GetPaginatedTestNavigationQuery
                {StepId = stepId, Page = query.Page, Size = query.Size});
            var testsWithLinks = tests.Select(test =>
                new HateoasResponse<TestNavigation>(test, GetLinksForTest(stepId, test.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                tests.ToPagedListResume(),
                testsWithLinks.ToList(),
                nameof(GetStepTests))
            );
        }

        [HttpDelete("{stepId:guid}", Name = nameof(DeleteStep))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStep(Guid stepId)
        {
            await Mediator.Send(new DeleteStepCommand(stepId));
            return NoContent();
        }

        public record UpdateStepCommandBody(
            string HeaderCode,
            string Name,
            string Description,
            int? MinFunctionsCount,
            int? MaxFunctionsCount,
            decimal Score,
            int Difficulty,
            Guid LanguageId);

        [HttpPut("{stepId:guid}", Name = nameof(UpdateStep))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateStep(Guid stepId, [FromBody] UpdateStepCommandBody command)
        {
            await Mediator.Send(new UpdateStepCommand(
                stepId,
                command.HeaderCode,
                command.Name,
                command.Description,
                command.MinFunctionsCount,
                command.MaxFunctionsCount,
                command.Score,
                command.Difficulty,
                command.LanguageId
            ));
            return NoContent();
        }

        #endregion

        #region Tests

        public record AddTestCommandBody(string OutputValidator, string InputGenerator, decimal Score);

        [HttpPost("{stepId:guid}/tests", Name = nameof(AddTest))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddTest(Guid stepId, [FromBody] AddTestCommandBody addTestCommand)
        {
            var (outputValidator, inputGenerator, score) = addTestCommand;
            var testId = await Mediator.Send(new AddTestCommand(stepId, outputValidator, inputGenerator, score));
            return CreatedAtRoute(
                nameof(TestsController.GetTestById),
                new {controller = nameof(TestsController).ControllerName(), testId}, null);
        }

        [HttpDelete("{stepId:guid}/tests/{testId:guid}", Name = nameof(DeleteTest))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTest(Guid stepId, Guid testId)
        {
            await Mediator.Send(new DeleteTestCommand(stepId, testId));
            return NoContent();
        }

        public record SetStepsTestsCommandBody(IList<StepTest> Tests);

        [HttpPut("{stepId:guid}/tests", Name = nameof(UpdateStepTests))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateStepTests(Guid stepId,
            [FromBody] SetStepsTestsCommandBody command)
        {
            await Mediator.Send(new SetStepTestsCommand(stepId, command.Tests));
            return NoContent();
        }

        #endregion

        #region Links

        private IList<LinkDto> GetLinksForStep(Guid stepId)
        {
            return new List<LinkDto>()
            {
                LinkDto.CreateLink(Url.Link(nameof(CreateStep), null)),
                LinkDto.SelfLink(Url.Link(nameof(GetStepById), new {stepId})),
                LinkDto.DeleteLink(Url.Link(nameof(DeleteStep), new {stepId})),
                LinkDto.UpdateLink(Url.Link(nameof(UpdateStep), new {stepId})),
                LinkDto.AllLink(Url.Link(nameof(GetSteps), null)),
                new(Url.Link(nameof(UpdateStepTests), new {stepId}), "set tests", HttpMethod.Put),
                new(Url.Link(nameof(AddTest), new {stepId}), "add test", HttpMethod.Post)
            };
        }

        private IList<LinkDto> GetLinksForTest(Guid stepId, Guid testId)
        {
            return new List<LinkDto>()
            {
                LinkDto.SelfLink(Url.Link(nameof(TestsController.GetTestById), new {testId})),
                LinkDto.CreateLink(Url.Link(nameof(AddTest), new {stepId, testId})),
                LinkDto.AllLink(Url.Link(nameof(GetStepTests), new {stepId}))
            };
        }

        #endregion
    }
}