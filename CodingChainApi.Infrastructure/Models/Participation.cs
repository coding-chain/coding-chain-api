using System;
using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Models
{
    public class Participation
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal CalculatedScore { get; set; }

        public bool Deactivated { get; set; }
        
        public Team Team { get; set; }
        public Step Step { get; set; }
        public Tournament Tournament { get; set; }
        public IList<Function> Functions { get; set; } = new List<Function>();
    }
}