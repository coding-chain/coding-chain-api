using System.Collections.Generic;
using Domain.StepEditions;
using Domain.Tournaments;

namespace Domain.Participations
{
    public class StepSessionEntity : StepEntity
    {
        public IList<TestEntity> Tests { get; set; }

        public StepSessionEntity(StepId id, IList<TournamentId> tournamentIds, IList<TestEntity> tests) :
            base(id, tournamentIds)
        {
            TournamentIds = tournamentIds;
            Tests = tests;
        }
    }
}