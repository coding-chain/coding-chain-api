using System;
using Domain.Contracts;

namespace Domain.Steps
{
    public record StepId(Guid Value) : IEntityId
    {
        public override string ToString() => Value.ToString();
    }
    
    public class StepAggregate: Aggregate<StepId>
    {
        public StepAggregate(StepId id) : base(id)
        {
        }
    }
}