using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Write.Contracts;
using Domain.StepEditions;
using MediatR;

namespace Application.Write.StepEditions
{
    public record AddTestCommand
        (Guid StepId,string Name, string OutputValidator, string InputGenerator, decimal Score) : IRequest<string>;

    public class AddTestHandler : IRequestHandler<AddTestCommand, string>
    {
        private readonly IStepEditionRepository _stepEditionRepository;

        public AddTestHandler(IStepEditionRepository stepEditionRepository)
        {
            _stepEditionRepository = stepEditionRepository;
        }

        public async Task<string> Handle(AddTestCommand request, CancellationToken cancellationToken)
        {
            var step = await _stepEditionRepository.FindByIdAsync(new StepId(request.StepId));
            if (step is null) throw new NotFoundException(request.StepId.ToString(), nameof(StepEditionAggregate));
            step.ValidateEdition();
            var newTest = new TestEntity(await _stepEditionRepository.GetNextTestIdAsync(), request.Name, request.OutputValidator,
                request.InputGenerator, request.Score);
            step.AddTest(newTest);
            await _stepEditionRepository.SetAsync(step);
            return newTest.Id.ToString();
        }
    }
}