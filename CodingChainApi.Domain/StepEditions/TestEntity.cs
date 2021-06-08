using System;
using Domain.Contracts;

namespace Domain.StepEditions
{
    public record TestId(Guid Value) : IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class TestEntity : Entity<TestId>
    {
        public TestEntity(TestId id, string name, string outputValidator, string inputGenerator, decimal score) :
            base(id)
        {
            Name = name;
            OutputValidator = outputValidator;
            InputGenerator = inputGenerator;
            Score = score;
        }

        public string Name { get; set; }
        public string OutputValidator { get; set; }
        public string InputGenerator { get; set; }
        public decimal Score { get; set; }
    }
}