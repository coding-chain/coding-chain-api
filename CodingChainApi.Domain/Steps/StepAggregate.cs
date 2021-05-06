using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.ProgrammingLanguages;
using Domain.Tournaments;

namespace Domain.Steps
{
    public record TestId(Guid Value) : IEntityId;

    public class TestEntity : Entity<TestId>
    {
        public string OutputValidator { get; set; }
        public string InputGenerator { get; set; }
        public decimal Score { get; set; }

        public TestEntity(TestId id, string outputValidator, string inputGenerator, decimal score) : base(id)
        {
            OutputValidator = outputValidator;
            InputGenerator = inputGenerator;
            Score = score;
        }
    }

    public record StepId(Guid Value) : IEntityId
    {
        public override string ToString() => Value.ToString();
    }

    public class StepAggregate : Aggregate<StepId>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int? MinFunctionCount { get; private set; }
        public int? MaxFunctionCount { get; private set; }
        public decimal Score { get; private set; }
        public int Difficulty { get; private set; }

        public ProgrammingLanguageId LanguageId { get; private set; }
        public IReadOnlyCollection<TestEntity> Tests => _tests.AsReadOnly();
        private List<TestEntity> _tests;

        public StepAggregate(StepId id, string name, string description, int? minFunctionCount, int? maxFunctionCount,
            decimal score, int difficulty, ProgrammingLanguageId languageId, List<TestEntity> tests) : base(id)
        {
            _tests = tests;
            LanguageId = languageId;
            Name = name;
            Description = description;
            MinFunctionCount = minFunctionCount;
            MaxFunctionCount = maxFunctionCount;
            Score = score;
            Difficulty = difficulty;
        }

        public void SetTests(IList<TestEntity> tests)
        {
            if (tests.Distinct().Count() != tests.Count)
                throw new DomainException("Tests contains duplicates");
            var errors = tests
                .Where(t => t.Score < 0)
                .Select(t => $"Test {t.Id} cannot have score {t.Score} bellow 0")
                .ToList();
            if (errors.Any())
                throw new DomainException(errors);
            this._tests = tests.ToList();
        }

        public void AddTest(TestEntity test)
        {
            if (test.Score < 0)
                throw new DomainException($"Test {test.Id} cannot have score {test.Score} bellow 0");
            if (_tests.Contains(test))
                throw new DomainException($"Test {test.Id} already in step tests");
            this._tests.Add(test);
        }

        public void RemoveTest(TestId id)
        {
            var test = _tests.FirstOrDefault(t => t.Id == id);
            if (test is null)
                throw new DomainException($"Test {id} not found");
            _tests.Remove(test);
        }

        public void SetScore(decimal score)
        {
            if (score < 0)
                throw new DomainException($"Score {score} is bellow 0");
            Score = score;
        }

        public void SetMinFunctionCnt(int minCnt)
        {
            if (minCnt >= MaxFunctionCount)
                throw new DomainException(
                    $"Minimal function count {minCnt} can't be greater than or equal to max function count {MaxFunctionCount}");
            MinFunctionCount = minCnt;
        }

        public void SetMaxFunctionCnt(int maxCnt)
        {
            if (maxCnt <= MinFunctionCount)
                throw new DomainException(
                    $"Maximal function count {maxCnt} can't be lesser than or equal to min function count {MinFunctionCount}");
            MaxFunctionCount = maxCnt;
        }
    }
}