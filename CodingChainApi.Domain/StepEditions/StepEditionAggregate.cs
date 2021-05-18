using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.ProgrammingLanguages;

namespace Domain.StepEditions
{
    public record StepId(Guid Value) : IEntityId
    {
        public override string ToString() => Value.ToString();
    }

    public class StepEditionAggregate : Aggregate<StepId>
    {
        public string HeaderCode { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int? MinFunctionsCount { get; private set; }
        public int? MaxFunctionsCount { get; private set; }
        public decimal Score { get; private set; }
        public int Difficulty { get; private set; }

        public bool IsPublished { get; private set; }

        public ProgrammingLanguageId LanguageId { get; private set; }
        public IReadOnlyCollection<TestEntity> Tests => _tests.ToList().AsReadOnly();
        private HashSet<TestEntity> _tests;

        private StepEditionAggregate(StepId id, string name, string description, string headerCode,
            ProgrammingLanguageId languageId) : base(id)
        {
            Id = id;
            Name = name;
            Description = description;
            HeaderCode = headerCode;
            LanguageId = languageId;
            _tests = new HashSet<TestEntity>();
        }

        private StepEditionAggregate(StepId id, string name, string description, string headerCode,
            int? minFunctionsCount,
            int? maxFunctionsCount,
            decimal score, int difficulty, bool isPublished, ProgrammingLanguageId languageId,
            List<TestEntity> tests) : base(id)
        {
            _tests = tests.ToHashSet();
            LanguageId = languageId;
            HeaderCode = headerCode;
            Name = name;
            Description = description;
            MinFunctionsCount = minFunctionsCount;
            MaxFunctionsCount = maxFunctionsCount;
            Score = score;
            Difficulty = difficulty;
            IsPublished = isPublished;
        }

        public static StepEditionAggregate CreateNew(StepId id, string name, string description, string headerCode,
            int? minFunctionCount, int? maxFunctionCount,
            decimal score, int difficulty, ProgrammingLanguageId languageId)
        {
            var step = new StepEditionAggregate(id, name, description, headerCode, languageId);
            step.SetMinMaxFunctionsCount(minFunctionCount, maxFunctionCount);
            step.SetScore(score);
            step.SetDifficulty(difficulty);
            return step;
        }

        public static StepEditionAggregate Restore(StepId id, string name, string description, string headerCode,
            int? minFunctionCount, int? maxFunctionCount,
            decimal score, int difficulty, bool isPublished, ProgrammingLanguageId languageId, List<TestEntity> tests)
        {
            return new StepEditionAggregate(id, name, description, headerCode, minFunctionCount, maxFunctionCount,
                score,
                difficulty, isPublished, languageId, tests);
        }

        public void ValidateEdition()
        {
            if (IsPublished)
                throw new DomainException("Cannot edit published step");
        }

        public void SetTests(IList<TestEntity> tests)
        {
            if (tests.Distinct().Count() != tests.Count)
                throw new DomainException("Tests contains duplicates");
            var errors = tests.SelectMany(t =>
            {
                try
                {
                    ValidateTest(t);
                }
                catch (DomainException e)
                {
                    return e.Errors;
                }

                return new List<string>();
            }).ToList();
            if (errors.Any())
                throw new DomainException(errors);
            this._tests = tests.ToHashSet();
        }

        public void ValidateTest(TestEntity test)
        {
            if (test.Score < 0)
                throw new DomainException($"Test {test.Id} cannot have score {test.Score} bellow 0");
        }

        public void UpdateTest(TestEntity test)
        {
            if (!_tests.Contains(test))
            {
                throw new DomainException($"Test {test.Id} doesn't exists");
            }
            ValidateTest(test);
            _tests.Remove(test);
            _tests.Add(test);
        }

        public void AddTest(TestEntity test)
        {
            ValidateTest(test);
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

        public void SetDifficulty(int difficulty)
        {
            if (difficulty < 0)
                throw new DomainException($"Difficulty {difficulty} is bellow 0");
            Difficulty = difficulty;
        }

        public void SetMinFunctionCnt(int minCnt)
        {
            ValidateMinFunctionCount(minCnt);
            if (minCnt >= MaxFunctionsCount)
                throw new DomainException(
                    $"Minimal function count {minCnt} can't be greater than or equal to max function count {MaxFunctionsCount}");
            MinFunctionsCount = minCnt;
        }

        public void SetMaxFunctionCnt(int maxCnt)
        {
            ValidateMaxFunctionCount(maxCnt);
            if (maxCnt <= MinFunctionsCount)
                throw new DomainException(
                    $"Maximal function count {maxCnt} can't be lesser than or equal to min function count {MinFunctionsCount}");
            MaxFunctionsCount = maxCnt;
        }

        public void ValidateMaxFunctionCount(int? maxCnt)
        {
            if (maxCnt < 0)
                throw new DomainException($"Maximal function count {maxCnt} is bellow 0");
        }

        public void ValidateMinFunctionCount(int? maxCnt)
        {
            if (maxCnt < 0)
                throw new DomainException($"Maximal function count {maxCnt} is bellow 0");
        }

        public void ValidateMinMaxFunctionCount(int? minCnt, int? maxCnt)
        {
            if (minCnt > maxCnt)
                throw new DomainException($"Minimal function count {minCnt} is greater than {maxCnt}");
        }

        public void SetMinMaxFunctionsCount(int? minCnt, int? maxCnt)
        {
            ValidateMinMaxFunctionCount(minCnt, maxCnt);
            MinFunctionsCount = minCnt;
            MaxFunctionsCount = maxCnt;
        }


        public void Update(string name, string description, string headerCode,
            int? minFunctionCount, int? maxFunctionCount,
            decimal score, int difficulty, ProgrammingLanguageId languageId)
        {
            Name = name;
            Description = description;
            HeaderCode = headerCode;
            LanguageId = languageId;
            SetMinMaxFunctionsCount(minFunctionCount, maxFunctionCount);
            SetScore(score);
            SetDifficulty(difficulty);
        }
    }
}