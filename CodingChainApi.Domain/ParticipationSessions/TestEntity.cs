using Domain.Contracts;
using Domain.StepEditions;

namespace Domain.ParticipationSessions
{
    public class TestEntity : Entity<TestId>
    {
        public TestEntity(TestId id, decimal score) : base(id)
        {
            Score = score;
        }

        public decimal Score { get; set; }
    }
}