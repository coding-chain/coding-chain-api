using System;

namespace CodingChainApi.Infrastructure.Models
{
    public class TournamentStep
    {
        public Guid Id { get; set; }
        public bool IsOptional { get; set; }
        public int Order { get; set; }
        
        public Tournament Tournament { get; set; }
        public Guid TournamentId {get; set; }
        public Step Step { get; set; }
        public Guid StepId { get; set; }

    }
}