using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Domain.ProgrammingLanguages;
using Domain.StepEditions;
using Domain.Steps;
using Domain.Tournaments;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class StepEditionRepository : IStepEditionRepository
    {
        private readonly CodingChainContext _context;

        public StepEditionRepository(CodingChainContext context)
        {
            _context = context;
        }

        private async Task<Step> GetModel(Guid stepId)
        {
            return await _context.Steps
                .Include(t => t.Tests)
                .Include(t => t.TournamentSteps)
                .ThenInclude( tS => tS.Tournament)
                .FirstOrDefaultAsync(t => !t.IsDeleted && stepId == t.Id) ?? new Step {Id = stepId};
        }

        private async Task<Step> ToModel(StepEditionAggregate aggregate)
        {
            var step = await GetModel(aggregate.Id.Value);
            var removedTests = GetRemovedTests(aggregate, step);
            removedTests.ForEach(test => test.IsDeleted = true);
            var currentTests = GetCurrentTests(step);
            var newTests = GetNewTests(aggregate, currentTests);
            currentTests.ForEach(currentTest =>
            {
                var test = aggregate.Tests.First(t => t.Id.Value == currentTest.Id);
                test.Score = currentTest.Score;
                test.InputGenerator = currentTest.InputGenerator;
                test.OutputValidator = currentTest.OutputValidator;
            });
            newTests.ForEach(test => step.Tests.Add(test));
            step.MinFunctionsCount = aggregate.MinFunctionsCount;
            step.MaxFunctionsCount = aggregate.MaxFunctionsCount;
            step.Description = aggregate.Description;
            step.Difficulty = aggregate.Difficulty;
            step.Name = aggregate.Name;
            step.Score = aggregate.Score;
            step.HeaderCode = aggregate.HeaderCode;
            step.ProgrammingLanguage = _context.ProgrammingLanguages.First(p => p.Id == aggregate.LanguageId.Value);
            return step;
        }

        private StepEditionAggregate ToAggregate(Step step) =>
            StepEditionAggregate.Restore(
                new StepId(step.Id),
                step.Name,
                step.Description,
                step.HeaderCode,
                step.MinFunctionsCount,
                step.MaxFunctionsCount,
                step.Score,
                step.Difficulty,
                step.TournamentSteps.Any(tS => tS.Tournament.IsPublished),
                new ProgrammingLanguageId(step.ProgrammingLanguage.Id),
                step.Tests.Select(ToTestEntity).ToList()
            );

        private TestEntity ToTestEntity(Test t) =>
            new TestEntity(new TestId(t.Id), t.OutputValidator, t.InputGenerator, t.Score);

        public async Task<StepId> SetAsync(StepEditionAggregate aggregate)
        {
            var step = await ToModel(aggregate);
            _context.Steps.Upsert(step);
            await _context.SaveChangesAsync();
            return new StepId(step.Id);
        }

        public async Task<StepEditionAggregate?> FindByIdAsync(StepId id)
        {
            var step = await _context.Steps
                .Include(s => s.Tests)
                .Include(s => s.ProgrammingLanguage)
                .FirstOrDefaultAsync(s => !s.IsDeleted && s.Id == id.Value);
            return step is null ? null : ToAggregate(step);
        }

        private static List<Test> GetNewTests(StepEditionAggregate aggregate, List<Test> currentTests)
        {
            return aggregate.Tests
                .Where(test => currentTests.All(t => t.Id != test.Id.Value))
                .Select(test => new Test
                    {Id = test.Id.Value ,Score = test.Score, InputGenerator = test.InputGenerator, OutputValidator = test.OutputValidator})
                .ToList();
        }

        private static List<Test> GetCurrentTests(Step step)
        {
            return step.Tests
                .Where(test => !test.IsDeleted)
                .ToList();
        }

        private static List<Test> GetRemovedTests(StepEditionAggregate aggregate, Step step)
        {
            return step.Tests
                .Where(test => aggregate.Tests.All(t => t.Id.Value != test.Id))
                .ToList();
        }

        public async Task RemoveAsync(StepId id)
        {
            var tournament = await _context.Steps
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id.Value);
            if (tournament is not null)
                tournament.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public Task<StepId> NextIdAsync() =>  new StepId(Guid.NewGuid()).ToTask();

        public Task<TestId> GetNextTestIdAsync() => new TestId(Guid.NewGuid()).ToTask();
    }
}