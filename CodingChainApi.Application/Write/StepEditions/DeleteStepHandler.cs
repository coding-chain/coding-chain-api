using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Write.Contracts;
using Domain.Steps;
using MediatR;

namespace Application.Write.StepEditions
{
    public record DeleteStepCommand(Guid StepId) : IRequest<string>;
    public class DeleteStepHandler: IRequestHandler<DeleteStepCommand, string>
    {
        private readonly IStepEditionRepository _stepEditionRepository;

        public DeleteStepHandler(IStepEditionRepository stepEditionRepository)
        {
            _stepEditionRepository = stepEditionRepository;
        }

        public async Task<string> Handle(DeleteStepCommand request, CancellationToken cancellationToken)
        {
            var step = await _stepEditionRepository.FindByIdAsync(new StepId(request.StepId));
            if (step is null) throw new NotFoundException(request.StepId.ToString(), nameof(StepEditionAggregate));
            step.ValidateEdition();
            await _stepEditionRepository.RemoveAsync(step.Id);
            return step.Id.ToString();
        }
    }
}