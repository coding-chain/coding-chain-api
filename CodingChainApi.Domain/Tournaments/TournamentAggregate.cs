using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.StepEditions;

namespace Domain.Tournaments
{
    public record TournamentId(Guid Value) : IEntityId
    {
        public override string ToString() => Value.ToString();
    }

    public class TournamentAggregate : Aggregate<TournamentId>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsPublished { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        private SortedSet<StepEntity> _steps;
        public IReadOnlyList<StepEntity> Steps => _steps.ToList().AsReadOnly();

        private class StepEntityComparer : IComparer<StepEntity>
        {
            public int Compare(StepEntity? x, StepEntity? y)
            {
                return x?.Order ?? 0 - y?.Order ?? 0;
            }
        }
        private TournamentAggregate(TournamentId id, string name, string description,
            bool isPublished, DateTime? startDate, DateTime? endDate, IList<StepEntity> steps) : base(id)
        {
            _steps = new SortedSet<StepEntity>(steps, new StepEntityComparer());
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            IsPublished = isPublished;
        }

        private TournamentAggregate(TournamentId id, string name, string description) : base(id)
        {
            Id = id;
            Name = name;
            Description = description;
            _steps = new SortedSet<StepEntity>();
        }

        public static TournamentAggregate Restore(TournamentId id, string name, string description,
            bool isPublished, DateTime? startDate, DateTime? endDate, IList<StepEntity> steps)
        {
            return new (id, name, description, isPublished, startDate, endDate, steps);
        }
        public static TournamentAggregate CreateNew(TournamentId id, string name, string description)
        {
            return new(id, name, description);
        }


        public void SetSteps(IList<StepEntity> steps)
        {
            var orderedSteps = new SortedSet<StepEntity>(steps);
            var errors = orderedSteps
                .Where((t, i) => t.Order != i + 1)
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
            if (_steps.Contains(stepEntity))
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
            var orderedSteps = _steps.ToList();
            for (var i = 0; i < orderedSteps.Count; i++)
            {
                orderedSteps[i].Order = i;
            }
            _steps = new SortedSet<StepEntity>(orderedSteps, new StepEntityComparer());
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
            if (date >= EndDate)
                throw new DomainException("End date couldn't be greater or equal than end date");
            StartDate = date;
        }

        public void SetStartDateAndEndDate(DateTime? startDate, DateTime? endDate)
        {
            if (IsPublished)
                throw new DomainException("Cannot change start or end date from published tournament");
            if(startDate is null && endDate is not null)
                throw new DomainException("Set start date first");
            if (startDate >= endDate)
                throw new DomainException("End date couldn't be lower or equal than start date");
            StartDate = startDate;
            EndDate = endDate;
        }

        public void Publish()
        {
            var errors = new List<string>();
            if (Steps.All(s => s.IsOptional))
                errors.Add("Tournament should contains mandatory steps");
            if (StartDate is null)
                errors.Add("Tournament should have start date");
            if (EndDate is null)
                errors.Add("Tournament should have end date");
            if (errors.Any())
                throw new DomainException(errors);
            IsPublished = true;
        }

        public void ValidateDeletion(DateTime nowTime)
        {
            if (IsPublished && EndDate > nowTime)
                throw new DomainException("Cannot delete published and not ended tournament");
        }

        public void Update(string name, string description, bool isPublished, DateTime? startDate, DateTime? endDate)
        {
            if (IsPublished)
                throw new DomainException("Cannot update published tournament");
            Name = name;
            Description = description;
            SetStartDateAndEndDate(startDate, endDate);
            if(isPublished)
                Publish();
        }


    }
}