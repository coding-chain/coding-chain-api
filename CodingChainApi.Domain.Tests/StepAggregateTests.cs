using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;
using Domain.ProgrammingLanguages;
using Domain.StepEditions;
using NUnit.Framework;

namespace CodingChainApi.Domain.Tests
{
    public class StepAggregateTests
    {
        private string _description;
        private int _difficulty;
        private string _headerCode;
        private StepId _id;
        private ProgrammingLanguageId _languageId;
        private string _name;
        private decimal _score;

        [SetUp]
        public void Setup()
        {
            _name = "Test step";
            _description = "Test description";
            _headerCode = "Test header";
            _id = new StepId(Guid.NewGuid());
            _difficulty = 4;
            _score = 42;
            _languageId = new ProgrammingLanguageId(Guid.NewGuid());
        }

        public TestEntity GetNewTest(decimal score)
        {
            return new(new TestId(Guid.NewGuid()), "Name", "Validator", "Generator", score);
        }

        public StepEditionAggregate GetNewStep(int? minFunctionCount = null, int? maxFunctionCount = null)
        {
            return StepEditionAggregate.Restore(_id, _name, _description, _headerCode, minFunctionCount,
                maxFunctionCount,
                _score, _difficulty,
                false,
                _languageId,
                new List<TestEntity>());
        }

        public StepEditionAggregate GetPublishedStep(int? minFunctionCount = null, int? maxFunctionCount = null)
        {
            return StepEditionAggregate.Restore(_id, _name, _description, _headerCode, minFunctionCount,
                maxFunctionCount,
                _score, _difficulty,
                true,
                _languageId,
                new List<TestEntity>());
        }

        [Test]
        public void create_new_step_should_work()
        {
            var step = GetNewStep();
            Assert.AreEqual(_name, step.Name);
            Assert.AreEqual(_description, step.Description);
            Assert.AreEqual(_id, step.Id);
            Assert.AreEqual(_difficulty, step.Difficulty);
            Assert.AreEqual(_score, step.Score);
            Assert.Null(step.MinFunctionsCount);
            Assert.Null(step.MaxFunctionsCount);
            CollectionAssert.IsEmpty(step.Tests);
        }

        // [Test]
        // public void set_tests_with_score_below_0_

        [Test]
        public void add_duplicated_test_should_throw()
        {
            var step = GetNewStep();
            var test = GetNewTest(10);
            step.AddTest(test);
            var duplicatedTest =
                new TestEntity(test.Id, test.Name, test.OutputValidator, test.InputGenerator, test.Score);
            Assert.Throws<DomainException>(() => step.AddTest(duplicatedTest));
        }

        [Test]
        public void add_test_with_score_bellow_0_should_throw()
        {
            var step = GetNewStep();
            var test = GetNewTest(-1);
            Assert.Throws<DomainException>(() => step.AddTest(test));
        }

        [Test]
        public void add_test_should_work()
        {
            var step = GetNewStep();
            var test = GetNewTest(12);
            step.AddTest(test);
            Assert.AreEqual(test, step.Tests.First());
        }

        [Test]
        public void remove_not_found_test_should_throw()
        {
            var step = GetNewStep();
            Assert.Throws<DomainException>(() => step.RemoveTest(new TestId(Guid.NewGuid())));
        }

        [Test]
        public void remove_test_should_work()
        {
            var step = GetNewStep();
            var test = GetNewTest(12);
            step.AddTest(test);
            step.RemoveTest(test.Id);
            CollectionAssert.DoesNotContain(step.Tests, test);
        }

        [Test]
        public void set_score_bellow_0_should_throw()
        {
            var step = GetNewStep();
            Assert.Throws<DomainException>(() => step.SetScore(-1));
        }

        [Test]
        public void set_score_should_work()
        {
            var step = GetNewStep();
            const int newScore = 123;
            step.SetScore(newScore);
            Assert.AreEqual(newScore, step.Score);
        }

        [Test]
        public void set_difficulty_bellow_0_should_throw()
        {
            var step = GetNewStep();
            Assert.Throws<DomainException>(() => step.SetDifficulty(-1));
        }

        [Test]
        public void set_difficulty_should_work()
        {
            var step = GetNewStep();
            const int newDifficulty = 4;
            step.SetScore(newDifficulty);
            Assert.AreEqual(newDifficulty, step.Difficulty);
        }

        [Test]
        public void set_min_function_count_greater_than_max_function_count_should_throw()
        {
            var step = GetNewStep(null, 1);
            Assert.Throws<DomainException>(() => step.SetMinFunctionCnt(3));
        }

        [Test]
        public void set_min_function_count_bellow_0_should_throw()
        {
            var step = GetNewStep();
            Assert.Throws<DomainException>(() => step.SetMinFunctionCnt(-1));
        }

        [Test]
        public void set_min_function_count_equals_to_max_function_count_should_throw()
        {
            var step = GetNewStep(null, 1);
            Assert.Throws<DomainException>(() => step.SetMinFunctionCnt(1));
        }

        [Test]
        public void validate_min_function_count_bellow_0_should_throw()
        {
            var step = GetNewStep();
            Assert.Throws<DomainException>(() => step.ValidateMinFunctionCount(-1));
        }

        [Test]
        public void set_max_function_count_bellow_than_min_function_count_should_throw()
        {
            var step = GetNewStep(3);
            Assert.Throws<DomainException>(() => step.SetMaxFunctionCnt(1));
        }

        [Test]
        public void set_max_function_count_bellow_0_should_throw()
        {
            var step = GetNewStep();
            Assert.Throws<DomainException>(() => step.SetMaxFunctionCnt(-1));
        }

        [Test]
        public void validate_max_function_count_bellow_0_should_throw()
        {
            var step = GetNewStep();
            Assert.Throws<DomainException>(() => step.ValidateMaxFunctionCount(-1));
        }

        [Test]
        public void set_max_function_count_equals_to_min_function_count_should_throw()
        {
            var step = GetNewStep(3);
            Assert.Throws<DomainException>(() => step.SetMaxFunctionCnt(3));
        }

        [Test]
        public void set_max_function_count_should_work()
        {
            var step = GetNewStep(1);
            const int maxCnt = 3;
            step.SetMaxFunctionCnt(maxCnt);
            Assert.AreEqual(maxCnt, step.MaxFunctionsCount);
        }

        [Test]
        public void set_min_function_count_should_work()
        {
            var step = GetNewStep(null, 3);
            const int minCnt = 1;
            step.SetMinFunctionCnt(minCnt);
            Assert.AreEqual(minCnt, step.MinFunctionsCount);
        }

        [Test]
        public void set_duplicated_tests_should_throw()
        {
            var step = GetNewStep();
            var test = GetNewTest(123);
            Assert.Throws<DomainException>(() => step.SetTests(new List<TestEntity> {test, test}));
        }

        [Test]
        public void set_tests_bellow_0_should_throw()
        {
            var step = GetNewStep();
            var failingTest = GetNewTest(-1);
            var test = GetNewTest(123);
            Assert.Throws<DomainException>(() => step.SetTests(new List<TestEntity> {test, failingTest}));
        }

        [Test]
        public void update_not_found_test_should_throw()
        {
            var step = GetNewStep();
            var test = GetNewTest(123);
            Assert.Throws<DomainException>(() => step.UpdateTest(test));
        }

        [Test]
        public void update_test_should_work()
        {
            var step = GetNewStep();
            var existingTest = GetNewTest(10);
            step.AddTest(existingTest);
            var modifications =
                new TestEntity(existingTest.Id, "Name", "TestOutput", "TestInput", existingTest.Score + 1);
            step.UpdateTest(modifications);
            var modifiedTest = step.Tests.First();
            Assert.AreEqual(modifiedTest.Score, modifications.Score);
            Assert.AreEqual(modifiedTest.OutputValidator, modifications.OutputValidator);
            Assert.AreEqual(modifiedTest.InputGenerator, modifications.InputGenerator);
        }


        [Test]
        public void set_tests_should_work()
        {
            var step = GetNewStep();
            var tests = new List<TestEntity> {GetNewTest(132), GetNewTest(123)};
            step.SetTests(tests);
            CollectionAssert.AreEqual(tests, step.Tests);
        }

        [Test]
        public void validate_edition_should_throw_on_published_step()
        {
            var step = GetPublishedStep();
            Assert.Throws<DomainException>(() => step.ValidateEdition());
        }

        [Test]
        public void validate_edition_should_work_on_not_published_step()
        {
            var step = GetNewStep();
            step.ValidateEdition();
            Assert.Pass();
        }

        [Test]
        public void validate_function_count_with_min_greater_than_max_should_throw()
        {
            var step = GetNewStep();
            Assert.Throws<DomainException>(() => step.ValidateMinMaxFunctionCount(4, 3));
        }

        [Test]
        public void set_min_and_max_function_count_with_min_greater_than_max_should_throw()
        {
            var step = GetNewStep();
            Assert.Throws<DomainException>(() => step.SetMinMaxFunctionsCount(4, 3));
        }

        [Test]
        public void set_min_and_max_function_count_should_work()
        {
            var expectedMin = 3;
            var expectedMax = 4;
            var step = GetNewStep();
            step.SetMinMaxFunctionsCount(expectedMin, expectedMax);
            Assert.AreEqual(expectedMax, step.MaxFunctionsCount);
            Assert.AreEqual(expectedMin, step.MinFunctionsCount);
        }
    }
}