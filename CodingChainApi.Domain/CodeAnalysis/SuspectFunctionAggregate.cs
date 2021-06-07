using System;
using System.Collections.Generic;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Participations;

namespace Domain.CodeAnalysis
{
    public class SuspectFunctionAggregate : Aggregate<FunctionId>
    {
        public IReadOnlyList<PlagiarizedFunctionEntity> PlagiarizedFunctions => _plagiarizedFunctions.AsReadOnly();
        private List<PlagiarizedFunctionEntity> _plagiarizedFunctions;

        public SuspectFunctionAggregate(FunctionId id, List<PlagiarizedFunctionEntity> plagiarizedFunctions) :
            base(id)
        {
            Id = id;
            _plagiarizedFunctions = plagiarizedFunctions;
        }

        public void AddPlagiarizedFunction(PlagiarizedFunctionEntity plagiarizedFunction)
        {
            if (_plagiarizedFunctions.Contains(plagiarizedFunction))
            {
                throw new DomainException(new List<string>()
                    {$"Function with id {plagiarizedFunction.Id} already in plagiarized pile"});
            }
            _plagiarizedFunctions.Add(plagiarizedFunction);
        }
    }
}