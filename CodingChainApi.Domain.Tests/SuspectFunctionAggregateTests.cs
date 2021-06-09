using System;
using System.Collections.Generic;
using System.Linq;
using Domain.CodeAnalysis;
using Domain.Exceptions;
using Domain.Participations;
using Domain.Users;
using NUnit.Framework;

namespace CodingChainApi.Domain.Tests
{
    public class SuspectFunctionAggregateTests
    {
        
        private double _suspectRate = 0.6;
        private SuspectFunctionAggregate GetEmptySuspectFunctionAggregate() =>
            new SuspectFunctionAggregate(TestsHelper.GetNewFunctionId(), new List<PlagiarizedFunctionEntity>());

        private PlagiarizedFunctionEntity GetNewPlagiarizedFunctionEntity() =>
            new (TestsHelper.GetNewFunctionId(),  _suspectRate, DateTime.Now);

        [Test]
        public void add_existing_function_should_throw()
        {
            var suspectFunction = GetEmptySuspectFunctionAggregate();
            var plagiarizedFunction = GetNewPlagiarizedFunctionEntity();
            suspectFunction.AddPlagiarizedFunction(plagiarizedFunction);
            var newFunc = new PlagiarizedFunctionEntity(plagiarizedFunction.Id, _suspectRate, DateTime.Now);
            Assert.Throws<DomainException>(() =>
            {
                suspectFunction.AddPlagiarizedFunction(newFunc);
            });
        }
    }
}