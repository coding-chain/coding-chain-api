using System;
using System.Collections.Generic;
using Domain.Contracts;
using Domain.Steps;

namespace Domain.Tournaments
{
    public record TournamentId(Guid Value) : IEntityId
    {
        public override string ToString() => Value.ToString();
    }

    public class StepEntity : Entity<StepId>
    {
        public bool IsOptional { get; set; }
        public int Order { get; set; }

        public StepEntity(StepId id, int order, bool isOptional) : base(id)
        {
            Order = order;
            IsOptional = isOptional;
        }
    }

    public class TournamentAggregate : Aggregate<TournamentId>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsPublished { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        private List<StepEntity> _steps;
        public IReadOnlyList<StepEntity> Steps => _steps.AsReadOnly();

        public TournamentAggregate(TournamentId id, string name, string description,
            bool isPublished, DateTime startDate, DateTime endDate, List<StepEntity> steps) : base(id)
        {
            _steps = steps;
            Name = name;
            Description = description;
            IsPublished = isPublished;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}