using System;
using System.Collections.Generic;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Participations;

namespace Domain.CodeAnalysis
{
    public class PlagiarizedFunctionAggregate : Aggregate<FunctionId>
    {
        public IReadOnlyList<PlagiarizedFunctionEntity> PlagiarizedFunction => _plagiarizedFunctions.AsReadOnly();
        private List<PlagiarizedFunctionEntity> _plagiarizedFunctions;

        public PlagiarizedFunctionAggregate(FunctionId id, List<PlagiarizedFunctionEntity> plagiarizedFunctions, FunctionId cheatingFunctionId) :
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