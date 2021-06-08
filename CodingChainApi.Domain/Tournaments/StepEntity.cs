using System;
using Domain.Contracts;
using Domain.StepEditions;

namespace Domain.Tournaments
{
    public class StepEntity : Entity<StepId>, IComparable<StepEntity>
    {
        public StepEntity(StepId id, int order, bool isOptional) : base(id)
        {
            Order = order;
            IsOptional = isOptional;
        }

        public bool IsOptional { get; set; }
        public int Order { get; set; }

        public int CompareTo(StepEntity? other)
        {
            return Order - other?.Order ?? 0;
        }
    }
}