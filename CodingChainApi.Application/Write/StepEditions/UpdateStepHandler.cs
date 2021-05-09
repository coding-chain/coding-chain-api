using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.ProgrammingLanguages;
using Domain.Steps;
using MediatR;

namespace Application.Write.StepEditions
{
    public record UpdateStepCommand(
        Guid StepId,
        string HeaderCode,
        string Name,
        string Description,
        int? MinFunctionsCount,
        int? MaxFunctionsCount,
        decimal Score,
        int Difficulty,
        Guid LanguageId) : IRequest<string>;

    public class UpdateStepHandler: IRequestHandler<UpdateStepCommand, string>
    {
        private readonly IStepEditionRepository _stepEditionRepository;
        private readonly IReadProgrammingLanguageRepository _languageRepository;

        public UpdateStepHandler(IStepEditionRepository stepEditionRepository, IReadProgrammingLanguageRepository languageRepository)
        {
            _stepEditionRepository = stepEditionRepository;
            _languageRepository = languageRepository;
        }

        public async Task<string> Handle(UpdateStepCommand request, CancellationToken cancellationToken)
        {
            if (!await _languageRepository.LanguageExistById(request.LanguageId))
                throw new NotFoundException(request.LanguageId.ToString(), "Language");
            var step = await _stepEditionRepository.FindByIdAsync(new StepId(request.StepId));
            if (step is null) throw new NotFoundException(request.StepId.ToString(), nameof(StepEditionAggregate));
            step.ValidateEdition();
            step.Update(request.Name, request.Description, request.HeaderCode, request.MinFunctionsCount,
                request.MaxFunctionsCount, request.Score, request.Difficulty, new ProgrammingLanguageId(request.LanguageId));
            await _stepEditionRepository.SetAsync(step);
            return step.Id.ToString();
        }
    }
}