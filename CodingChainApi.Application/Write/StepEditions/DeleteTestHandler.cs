using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Write.Contracts;
using Domain.StepEditions;
using Domain.Steps;
using MediatR;

namespace Application.Write.StepEditions
{
    public record DeleteTestCommand(Guid StepId, Guid TestId): IRequest<string>;
    public class DeleteTestHandler: IRequestHandler<DeleteTestCommand, string>
    {
        private readonly IStepEditionRepository _stepEditionRepository;

        public DeleteTestHandler(IStepEditionRepository stepEditionRepository)
        {
            _stepEditionRepository = stepEditionRepository;
        }

        public async Task<string> Handle(DeleteTestCommand request, CancellationToken cancellationToken)
        {
            var step = await _stepEditionRepository.FindByIdAsync(new StepId(request.StepId));
            if (step is null) throw new NotFoundException(request.StepId.ToString(), nameof(StepEditionAggregate));
            step.ValidateEdition();
            step.RemoveTest(new TestId(request.TestId));
            await _stepEditionRepository.SetAsync(step);
            return step.Id.ToString();
        }
    }
}