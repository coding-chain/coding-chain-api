using System;
using System.Collections.Generic;
using Domain.CodeAnalysis;
using Domain.Participations;

namespace CodingChainApi.Infrastructure.Models
{
    public class PlagiarizedFunction
    {
        public Guid Id { get; set; }
        public FunctionId CheatingFunctionId { get; set; }
        public IList<PlagiarizedFunctionEntity> PlagiarizedFunctions { get; set; } = new List<PlagiarizedFunctionEntity>();

        public bool IsDeleted { get; set; }
    }
}