using System;

namespace CodingChainApi.Infrastructure.Models
{
    public class PlagiarismFunction
    {
        public Guid Id { get; set; }
        public Guid PlagiarizedFunctionId { get; set; }
        public Guid CheatingFunctionId { get; set; }

        public Function PlagiarizedFunction { get; set; }

        public Function CheatingFunction { get; set; }

        public DateTime DetectionDate { get; set; }

        public double Rate { get; set; }
        public bool IsDeleted { get; set; }
    }
}