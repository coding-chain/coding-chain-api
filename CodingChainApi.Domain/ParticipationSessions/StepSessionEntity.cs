using System.Collections.Generic;
using Domain.StepEditions;
using Domain.Tournaments;
using StepEntity = Domain.Participations.StepEntity;

namespace Domain.ParticipationSessions
{
    public class StepSessionEntity : StepEntity
    {
        public StepSessionEntity(StepId id, IList<TournamentId> tournamentIds, IList<TestEntity> tests) :
            base(id, tournamentIds)
        {
            TournamentIds = tournamentIds;
            Tests = tests;
        }

        public IList<TestEntity> Tests { get; set; }
    }
}