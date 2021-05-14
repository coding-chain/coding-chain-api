using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingChainApi.Infrastructure.Models
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDeleted { get; set; }

        public IList<Participation> Participations = new List<Participation>();
        public IList<TournamentStep> TournamentSteps = new List<TournamentStep>();

        public IList<Guid> StepsIds => TournamentSteps
            .Where(tS => !tS.Step.IsDeleted)
            .Select(tS => tS.StepId)
            .ToList();

        public IList<Guid> ParticipationsIds => Participations
            .Where(p => !p.Team.IsDeleted)
            .Select(p => p.Id)
            .ToList();
    }
}