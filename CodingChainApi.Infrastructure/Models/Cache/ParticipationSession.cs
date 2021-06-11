using System;
using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Models.Cache
{
    public class ParticipationSession
    {
        public Guid Id { get; set; }
        public IList<Guid> PassedTestsIds { get; set; } = new List<Guid>();
        public Team Team { get; set; }
        public Step Step { get; set; }
        public string? LastError { get; set; }
        public string? LastOutput { get; set; }
        public bool IsReady { get; set; }
        public DateTime? ProcessStartTime { get; set; }
        public Tournament Tournament { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal CalculatedScore { get; set; }
        public IList<Function> Functions { get; set; } = new List<Function>();
    }
}