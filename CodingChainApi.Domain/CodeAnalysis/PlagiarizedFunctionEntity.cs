using System;
using Domain.Contracts;
using Domain.Participations;

namespace Domain.CodeAnalysis
{
    public class PlagiarizedFunctionEntity : Entity<FunctionId>
    {
        public PlagiarizedFunctionEntity(FunctionId id, double rate, DateTime detectionDate) : base(id)
        {
            Id = id;
            Rate = rate;
        }

        public double Rate { get; set; }
        public DateTime DetectionDate { get; set; }
    }
}