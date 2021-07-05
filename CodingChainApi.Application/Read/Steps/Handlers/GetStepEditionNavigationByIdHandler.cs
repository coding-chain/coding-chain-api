using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Read.Contracts;
using MediatR;

namespace Application.Read.Steps.Handlers
{
    public record GetStepNavigationByIdQuery(Guid StepId) : IRequest<StepNavigation>;

    public class GetStepEditionNavigationByIdHandler : IRequestHandler<GetStepNavigationByIdQuery, StepNavigation>
    {
        private readonly IReadStepRepository _readStepRepository;

        public GetStepEditionNavigationByIdHandler(IReadStepRepository readStepRepository)
        {
            _readStepRepository = readStepRepository;
        }

        public async Task<StepNavigation> Handle(GetStepNavigationByIdQuery request,
            CancellationToken cancellationToken)
        {
            var step = await _readStepRepository.GetOneStepNavigationById(request.StepId);
            if (step is null)
                throw new ApplicationException($"Step with id {request.StepId} not found");
            return step;
        }
    }
}