using System;
using Domain.Contracts;
using Domain.Participations;

namespace Domain.CodeAnalysis
{
    public class PlagiarizedFunctionEntity : Entity<FunctionId>
    {
        public double Rate { get; set; }


        public PlagiarizedFunctionEntity(FunctionId id, double rate) : base(id)
        {
            Id = id;
            Rate = rate;
        }
    }
}