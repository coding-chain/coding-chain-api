using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;
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
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        private List<StepEntity> _steps;
        public IReadOnlyList<StepEntity> Steps => _steps.AsReadOnly();

        public TournamentAggregate(TournamentId id, string name, string description,
            bool isPublished, DateTime? startDate, DateTime? endDate, IList<StepEntity> steps) : base(id)
        {
            _steps = steps.OrderBy(s => s.Order).ToList();
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = EndDate;
            IsPublished = isPublished;
        }
        
        public void SetSteps(IList<StepEntity> steps)
        {
            var orderedSteps = steps.OrderBy(s => s.Order).ToList();
            var errors = orderedSteps
                .Where((t, i) => t.Order != i)
                .Select(t => $"Step with id {t.Id} and order {t.Order} isn't ordered")
                .ToList();
            if (errors.Any())
                throw new DomainException(errors);
            _steps = orderedSteps;
        }

        public void AddStep(StepEntity stepEntity)
        {
            if (IsPublished)
                throw new DomainException("Cannot add step from published tournament");
            if (_steps.Any(s => s.Id == stepEntity.Id))
                throw new DomainException($"Step {stepEntity.Id} already in steps");
            _steps.Add(stepEntity);
            ReorderSteps();
        }

        public void RemoveStep(StepId id)
        {
            if (IsPublished)
                throw new DomainException("Cannot remove step from published tournament");
            var stepToRemove = Steps.FirstOrDefault(s => s.Id == id);
            if (stepToRemove is null)
                throw new DomainException($"Step {id} not found in tournament {Id}");
            _steps.Remove(stepToRemove);
            ReorderSteps();
        }

        private void ReorderSteps()
        {
            var orderedSteps = _steps.OrderBy(s => s.Order).ToList();
            for (var i = 0; i < orderedSteps.Count; i++)
            {
                orderedSteps[i].Order = i;
            }
        }

        public void SetEndDate(DateTime date)
        {
            if (IsPublished)
                throw new DomainException("Cannot change end date from published tournament");
            if (StartDate is null)
                throw new DomainException("Set start date first");
            if (date <= StartDate)
                throw new DomainException("End date couldn't be lower or equal than start date");
            EndDate = date;
        }

        public void SetStartDate(DateTime date)
        {
            if (IsPublished)
                throw new DomainException("Cannot change start date from published tournament");
            if(date >= EndDate)
                throw new DomainException("End date couldn't be greater or equal than end date");
            StartDate = date;
        }

        public void Publish()
        {
            var errors = new List<string>();
            if(Steps.All(s => s.IsOptional))
                errors.Add("Tournament should contains mandatory steps");
            if(StartDate is null)
                errors.Add("Tournament should have start date");
            if(EndDate is null)
                errors.Add("Tournament should have end date");
            if (errors.Any())
                throw new DomainException(errors);
            IsPublished = true;
        }
        
    }
}