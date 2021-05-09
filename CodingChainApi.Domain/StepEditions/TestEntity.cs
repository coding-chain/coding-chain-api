using System;
using Domain.Contracts;

namespace Domain.StepEditions
{
    public record TestId(Guid Value) : IEntityId
    {
        public override string ToString() => Value.ToString();
    }

    public class TestEntity : Entity<TestId>
    {
        public string OutputValidator { get; set; }
        public string InputGenerator { get; set; }
        public decimal Score { get; set; }

        public TestEntity(TestId id, string outputValidator, string inputGenerator, decimal score) : base(id)
        {
            OutputValidator = outputValidator;
            InputGenerator = inputGenerator;
            Score = score;
        }
    }
}