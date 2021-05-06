using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;
using Domain.ProgrammingLanguages;
using Domain.Steps;
using NUnit.Framework;

namespace CodingChainApi.Domain.Tests
{
    public class StepAggregateTests
    {
        private string _name;
        private string _description;
        private StepId _id;
        private int _difficulty;
        private decimal _score;
        private ProgrammingLanguageId _languageId;

        [SetUp]
        public void Setup()
        {
            _name = "Test step";
            _description = "Test description";
            _id = new StepId(Guid.NewGuid());
            _difficulty = 4;
            _score = 42;
            _languageId = new ProgrammingLanguageId(Guid.NewGuid());
        }

        public TestEntity GetNewTest(decimal score) =>
            new TestEntity(new TestId(Guid.NewGuid()), "Validator", "Generator", score);

        public StepAggregate GetNewStep(int? minFunctionCount = null, int? maxFunctionCount = null) =>
            new(_id, _name, _description, minFunctionCount, maxFunctionCount, _score, _difficulty, _languageId,
                new List<TestEntity>());

        [Test]
        public void create_new_step_should_work()
        {
            var step = GetNewStep();
            Assert.AreEqual(_name, step.Name);
            Assert.AreEqual(_description, step.Description);
            Assert.AreEqual(_id, step.Id);
            Assert.AreEqual(_difficulty, step.Difficulty);
            Assert.AreEqual(_score, step.Score);
            Assert.Null(step.MinFunctionCount);
            Assert.Null(step.MaxFunctionCount);
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
            var duplicatedTest = new TestEntity(test.Id, test.OutputValidator, test.InputGenerator, test.Score);
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
        public void set_min_function_count_greater_than_max_function_count_should_throw()
        {
            var step = GetNewStep(null, 1);
            Assert.Throws<DomainException>(() => step.SetMinFunctionCnt(3));
        }

        [Test]
        public void set_min_function_count_equals_to_max_function_count_should_throw()
        {
            var step = GetNewStep(null, 1);
            Assert.Throws<DomainException>(() => step.SetMinFunctionCnt(1));
        }

        [Test]
        public void set_max_function_count_bellow_than_min_function_count_should_throw()
        {
            var step = GetNewStep(3, null);
            Assert.Throws<DomainException>(() => step.SetMaxFunctionCnt(1));
        }

        [Test]
        public void set_max_function_count_equals_to_min_function_count_should_throw()
        {
            var step = GetNewStep(3, null);
            Assert.Throws<DomainException>(() => step.SetMaxFunctionCnt(3));
        }

        [Test]
        public void set_max_function_count_should_work()
        {
            var step = GetNewStep(1, null);
            const int maxCnt = 3;
            step.SetMaxFunctionCnt(maxCnt);
            Assert.AreEqual(maxCnt, step.MaxFunctionCount);
        }

        [Test]
        public void set_min_function_count_should_work()
        {
            var step = GetNewStep(null, 3);
            const int minCnt = 1;
            step.SetMinFunctionCnt(minCnt);
            Assert.AreEqual(minCnt, step.MinFunctionCount);
        }

        [Test]
        public void set_duplicated_tests_should_throw()
        {
            var step = GetNewStep();
            var test = GetNewTest(123);
            Assert.Throws<DomainException>(() => step.SetTests(new List<TestEntity>() {test, test}));
        }
        
        [Test]
        public void set_tests_bellow_0_should_throw()
        {
            var step = GetNewStep();
            var failingTest = GetNewTest(-1);
            var test = GetNewTest(123);
            Assert.Throws<DomainException>(() => step.SetTests(new List<TestEntity>() {test, failingTest}));
        }

        [Test]
        public void set_tests_should_work()
        {
            var step = GetNewStep();
            var tests = new List<TestEntity>() {GetNewTest(132), GetNewTest(123)};
            step.SetTests(tests);
            CollectionAssert.AreEqual(tests, step.Tests);
        }
    }
}