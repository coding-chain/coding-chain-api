using System;
using System.Threading.Tasks;
using Application.Records;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeosCodingApi.Services;

namespace NeosCodingApi.Controllers
{
    public class CodeController : ApiControllerBase
    {
        public CodeController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) : base(
            mediator, mapper, propertyCheckerService)
        {
        }

        [HttpPost(TemplateActionName, Name = nameof(Execute))]
        public async Task<IActionResult> Execute(ICodeExecutionRecords.RunParticipationTestsCommand command)
        {
            var res = await Mediator.Send(command);
            Console.WriteLine(command.ToString());
            return Ok();
        }
    }
}