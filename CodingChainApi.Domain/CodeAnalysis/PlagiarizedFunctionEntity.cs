using System;
using Domain.Contracts;
using Domain.Participations;

namespace Domain.CodeAnalysis
{
    public class PlagiarizedFunctionEntity : Entity<FunctionId>
    {
        public PlagiarizedFunctionEntity(FunctionId id, double rate, string hash, string suspectHash,
            DateTime detectionDate, bool? isValid = null) : base(id)
        {
            Id = id;
            Rate = rate;
            Hash = hash;
            SuspectHash = suspectHash;
            DetectionDate = detectionDate;
        }

        public bool? IsValid { get; set; }
        public string Hash { get; set; }
        public string SuspectHash { get; set; }
        public double Rate { get; set; }
        public DateTime DetectionDate { get; set; }
    }
}