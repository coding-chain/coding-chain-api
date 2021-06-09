using AutoMapper;
using MediatR;
using NeosCodingApi.Services;

namespace NeosCodingApi.Controllers
{
    public class CodeController : ApiControllerBase
    {
        public CodeController(ISender mediator, IMapper mapper, IPropertyCheckerService propertyCheckerService) : base(
            mediator, mapper, propertyCheckerService)
        {
        }

        // [HttpPost(Name = nameof(Execute))]
        // public async Task<IActionResult> Execute(RunParticipationTestsCommand command)
        // {
        //     var res = await Mediator.Send(command);
        //     res.ProcessEnded += (sender, args) => Console.WriteLine($"output: {args.Output}, erros: {args.Error}");
        //     Console.WriteLine(command.ToString());
        //     return Ok();
        // }   
    }
}