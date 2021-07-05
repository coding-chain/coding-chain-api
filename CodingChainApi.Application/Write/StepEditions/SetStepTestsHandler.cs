using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.StepEditions;
using MediatR;

namespace Application.Write.StepEditions
{
    public record StepTest(Guid? Id, string Name, string OutputValidator, string InputGenerator, decimal Score);

    public record SetStepTestsCommand(Guid StepId, IList<StepTest> Tests) : IRequest<string>;

    public class SetStepTestsHandler : IRequestHandler<SetStepTestsCommand, string>
    {
        private readonly IReadTestRepository _readTestRepository;
        private readonly IStepEditionRepository _stepEditionRepository;

        public SetStepTestsHandler(IStepEditionRepository stepEditionRepository, IReadTestRepository readTestRepository)
        {
            _stepEditionRepository = stepEditionRepository;
            _readTestRepository = readTestRepository;
        }

        public async Task<string> Handle(SetStepTestsCommand request, CancellationToken cancellationToken)
        {
            var step = await _stepEditionRepository.FindByIdAsync(new StepId(request.StepId));
            if (step is null)
                throw new NotFoundException(request.StepId.ToString(), "Step");
            var tests = request.Tests
                .Where(t => t.Id is not null)
                .Select(t =>
                    new TestEntity(new TestId(t.Id!.Value), t.Name, t.OutputValidator, t.InputGenerator, t.Score))
                .ToList();
            var newTests = request.Tests.Where(t => t.Id is null);
            tests.ForEach(existingTest =>
            {
                if (!step.Tests.Contains(existingTest))
                    throw new NotFoundException(existingTest.Id.ToString(), "Test");
            });
            foreach (var newTest in newTests)
                tests.Add(new TestEntity(await _stepEditionRepository.GetNextTestIdAsync(), newTest.Name,
                    newTest.OutputValidator,
                    newTest.InputGenerator, newTest.Score));
            step.SetTests(tests);
            return (await _stepEditionRepository.SetAsync(step)).ToString();
        }
    }
}