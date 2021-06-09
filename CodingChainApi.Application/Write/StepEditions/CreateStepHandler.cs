using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.ProgrammingLanguages;
using Domain.StepEditions;
using MediatR;

namespace Application.Write.StepEditions
{
    public record CreateStepCommand(Guid LanguageId, string HeaderCode, string Name, string Description, decimal Score,
        int Difficulty, int? MinFunctionsCount = null, int? MaxFunctionsCount = null) : IRequest<string>;

    public class CreateStepHandler : IRequestHandler<CreateStepCommand, string>
    {
        private readonly IReadProgrammingLanguageRepository _languageRepository;
        private readonly IStepEditionRepository _stepEditionRepository;

        public CreateStepHandler(IStepEditionRepository stepEditionRepository,
            IReadProgrammingLanguageRepository languageRepository)
        {
            _stepEditionRepository = stepEditionRepository;
            _languageRepository = languageRepository;
        }

        public async Task<string> Handle(CreateStepCommand request, CancellationToken cancellationToken)
        {
            if (!await _languageRepository.LanguageExistById(request.LanguageId))
                throw new ApplicationException($"Language {request.LanguageId} not found");
            var step = StepEditionAggregate.CreateNew(
                await _stepEditionRepository.NextIdAsync(),
                request.Name,
                request.Description,
                request.HeaderCode,
                request.MinFunctionsCount,
                request.MaxFunctionsCount,
                request.Score,
                request.Difficulty,
                new ProgrammingLanguageId(request.LanguageId));
            var id = await _stepEditionRepository.SetAsync(step);
            return id.ToString();
        }
    }
}