using System;
using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Models
{
    public class Step
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? MinFunctionsCount { get; set; }
        public int? MaxFunctionsCount { get; set; }
        public decimal Score { get; set; }
        public bool IsOptional { get; set; }
        public int Difficulty { get; set; }
        public bool IsDeleted { get; set; }
        
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
        public IList<Test> Tests { get; set; } = new List<Test>();
        public IList<TournamentStep> TournamentSteps = new List<TournamentStep>();
        public IList<Participation> Participations { get; set; } = new List<Participation>();
    }
}