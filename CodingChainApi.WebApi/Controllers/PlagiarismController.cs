using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Plagiarism;
using Application.Read.Plagiarism.Handlers;
using Application.Write.Plagiarism;
using AutoMapper;
using CodingChainApi.Helpers;
using CodingChainApi.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using NSwag.Annotations;

namespace CodingChainApi.Controllers
{
    public class PlagiarismController : ApiControllerBase
    {
        public PlagiarismController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) :
            base(mediator, mapper, propertyCheckerService)
        {
        }


        [HttpPost("functions/{functionId:guid}/analyze", Name = nameof(StartAnalyzeForFunction))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> StartAnalyzeForFunction(Guid functionId)
        {
            await Mediator.Send(new PlagiarismAnalyseCommand(functionId));
            return Accepted();
        }

        [HttpGet("functions", Name = nameof(GetPaginatedSuspectFunctionsFiltered))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<SuspectFunctionNavigation>>))]
        public async Task<IActionResult> GetPaginatedSuspectFunctionsFiltered([FromQuery] GetSuspectFunctionsPaginatedQuery query)
        {
            var functions = await Mediator.Send(query);
            var functionsWithLinks = functions.Select(function =>
                new HateoasResponse<SuspectFunctionNavigation>(function, GetLinksForFunction(function.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                functions.ToPagedListResume(),
                functionsWithLinks.ToList(),
                nameof(GetPaginatedSuspectFunctionsFiltered))
            );
        }

        [HttpGet("functions/{functionId:guid}", Name = nameof(GetOneSuspectFunction))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasResponse<SuspectFunctionNavigation>))]
        public async Task<IActionResult> GetOneSuspectFunction(Guid functionId)
        {
            var function = await Mediator.Send(new GetOneSuspectFunctionQuery(functionId));
            var functionWithLinks =
                new HateoasResponse<SuspectFunctionNavigation>(function, GetLinksForFunction(function.Id));
            return Ok(functionWithLinks);
        }

        public record GetLastPlagiarizedFunctionsByFunctionQueryParams : PaginationQueryBase;

        [HttpGet("functions/{functionId:guid}/plagiarized", Name = nameof(GetPlagiarizedFunctions))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(HateoasPageResponse<HateoasResponse<FunctionCodeNavigation>>))]
        public async Task<IActionResult> GetPlagiarizedFunctions(Guid functionId,
            GetLastPlagiarizedFunctionsByFunctionQueryParams query)
        {
            var functions = await Mediator.Send(new GetLastPlagiarizedFunctionsByFunctionQuery(functionId)
                {Page = query.Page, Size = query.Size});
            var functionsWithLinks = functions.Select(function =>
                new HateoasResponse<FunctionCodeNavigation>(function, GetLinksForFunction(function.Id)));
            return Ok(HateoasResponseBuilder.FromPagedList(
                Url,
                functions.ToPagedListResume(),
                functionsWithLinks.ToList(),
                nameof(GetPlagiarizedFunctions))
            );
        }

        private IList<LinkDto> GetLinksForFunction(Guid functionId)
        {
            return new List<LinkDto>
            {
                new(Url.Link(nameof(StartAnalyzeForFunction), new {functionId}), "analyze", HttpMethod.Post),
                LinkDto.SelfLink(Url.Link(nameof(GetOneSuspectFunction), new {functionId})),
                LinkDto.AllLink(Url.Link(nameof(GetPaginatedSuspectFunctionsFiltered), null)),
                LinkDto.AllLink(Url.Link(nameof(GetPlagiarizedFunctions), new {functionId}))
            };
        }
    }
}