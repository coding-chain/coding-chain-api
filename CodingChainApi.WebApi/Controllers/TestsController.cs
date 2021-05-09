using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Read.Steps;
using Application.Read.Steps.Handlers;
using Application.Read.Tests;
using Application.Read.Tests.Handlers;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeosCodingApi.Helpers;
using NeosCodingApi.Services;
using NSwag.Annotations;

namespace NeosCodingApi.Controllers
{
    public class TestsController : ApiControllerBase
    {
        public TestsController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) : base(
            mediator, mapper, propertyCheckerService)
        {
        }

        #region Tests

        [HttpGet("{testId:guid}", Name = nameof(GetTestById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<TestNavigation>))]
        public async Task<IActionResult> GetTestById(Guid testId)
        {
            var test = await Mediator.Send(new GetTestNavigationByIdQuery(testId));
            var testWithLinks = new HateoasResponse<TestNavigation>(test, GetLinksForTest(test.StepId, testId));
            return Ok(testWithLinks);
        }

        [HttpGet(Name = nameof(GetTests))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<IList<HateoasResponse<TestNavigation>>>))]
        public async Task<IActionResult> GetTests([FromQuery] GetPaginatedTestNavigationQuery query)
        {
            var test = await Mediator.Send(query);
            var testWithLinks = test.Select(test =>
                new HateoasResponse<TestNavigation>(test, GetLinksForTest(test.StepId, test.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                test.ToPagedListResume(),
                testWithLinks.ToList(),
                nameof(GetTests))
            );
        }

        #endregion

        #region Links
        private IList<LinkDto> GetLinksForTest(Guid stepId, Guid testId)
        {
            return new List<LinkDto>()
            {
                LinkDto.SelfLink(Url.Link(nameof(GetTestById), new {testId})),
                LinkDto.DeleteLink(Url.Link(nameof(StepsController.AddTest), new {stepId, testId}))
            };
        }
        

        #endregion
       
    }
}