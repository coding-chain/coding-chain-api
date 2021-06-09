using System.Collections.Generic;
using Domain.Contracts;
using Domain.StepEditions;
using Domain.Tournaments;

namespace Domain.Participations
{
    public class StepEntity : Entity<StepId>
    {
        public StepEntity(StepId id, IList<TournamentId> tournamentIds) :
            base(id)
        {
            TournamentIds = tournamentIds;
        }

        public IList<TournamentId> TournamentIds { get; set; }
    }
}