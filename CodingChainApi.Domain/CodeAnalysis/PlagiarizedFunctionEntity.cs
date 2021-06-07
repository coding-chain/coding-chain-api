using System;
using Domain.Contracts;
using Domain.Participations;

namespace Domain.CodeAnalysis
{
    public class PlagiarizedFunctionEntity : Entity<FunctionId>
    {
        public FunctionId CheatingFunctionId;
        public FunctionId ComparedFunctionId;
        public double Rate { get; set; }

        public int CompareTo(PlagiarizedFunctionEntity? other)
        {
            if (other?.Id == Id) return 0;
            return -1;
        }

        public PlagiarizedFunctionEntity(FunctionId id, FunctionId cheatingFunctionId, FunctionId comparedFunctionId,
            double rate) : base(id)
        {
            Id = id;
            CheatingFunctionId = cheatingFunctionId;
            ComparedFunctionId = comparedFunctionId;
            Rate = rate;
        }
    }
}