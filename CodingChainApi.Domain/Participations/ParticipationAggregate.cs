using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Teams;
using Domain.Tournaments;

namespace Domain.Participations
{
    public record ParticipationId(Guid Value) : IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class ParticipationAggregate : Aggregate<ParticipationId>
    {
        public TeamEntity Team { get; private set; }
        public TournamentEntity TournamentEntity { get; private set; }
        public StepEntity StepEntity { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public decimal CalculatedScore { get; private set; }
        public IReadOnlyList<FunctionEntity> Functions => _functions.ToList().AsReadOnly();
        private SortedSet<FunctionEntity> _functions;


        public static ParticipationAggregate CreateNew(ParticipationId id, TeamEntity team,
            TournamentEntity tournamentEntity,
            StepEntity stepEntity, DateTime startDate)
        {
            var errors = new List<string>();
            if (!stepEntity.TournamentIds.Contains(tournamentEntity.Id))
                errors.Add(
                    $"Cannot create new participation on step {stepEntity.Id} and tournament {tournamentEntity.Id} because step is not linked to tournament");
            if (!tournamentEntity.IsPublished)
                errors.Add($"Cannot create participation on not published tournament {tournamentEntity.Id}");

            if (errors.Any())
                throw new DomainException(errors);

            return new ParticipationAggregate(id, team, tournamentEntity, stepEntity, startDate, null, 0,
                new List<FunctionEntity>());
        }

        public static ParticipationAggregate Restore(ParticipationId id, TeamEntity team,
            TournamentEntity tournamentEntity,
            StepEntity stepEntity, DateTime startDate, DateTime? endDate, decimal calculatedScore,
            IList<FunctionEntity> functions)
        {
            return new(id, team, tournamentEntity, stepEntity, startDate, endDate, calculatedScore,
                functions);
        }

        private ParticipationAggregate(ParticipationId id, TeamEntity team, TournamentEntity tournamentEntity,
            StepEntity stepEntity,
            DateTime startDate, DateTime? endDate, decimal calculatedScore, IList<FunctionEntity> functions) : base(id)
        {
            Team = team;
            TournamentEntity = tournamentEntity;
            StepEntity = stepEntity;
            _functions = new SortedSet<FunctionEntity>(functions);
            StartDate = startDate;
            EndDate = endDate;
            CalculatedScore = calculatedScore;
        }

        public void SetFunctions(IList<FunctionEntity> functions)
        {
            var orderedFunctions = new SortedSet<FunctionEntity>(functions);
            var errors = orderedFunctions
                .SelectMany((function, i) =>
                {
                    var subErrors = new List<string>();
                    if (function.Order is not null && function.Order != i + 1)
                        subErrors.Add($"Function {function.Id} and order {function.Order} isn't ordered");
                    var sameFunction = _functions.FirstOrDefault(oldFunction => function.Id == oldFunction.Id);
                    if (sameFunction?.LastModificationDate > function.LastModificationDate)
                        subErrors.Add($"Function {function.Id} have last modification date lower than before");
                    try
                    {
                        ValidateFunction(function);
                    }
                    catch (DomainException e)
                    {
                        subErrors.AddRange(e.Errors);
                    }
                    return subErrors;
                })
                .ToList();

            if (errors.Any())
                throw new DomainException(errors);
            _functions = orderedFunctions;
        }

        public void ValidateFunction(FunctionEntity function)
        {
            if (!Team.UserIds.Contains(function.UserId))
                throw new DomainException(
                    $"User {function.UserId} for function  {function.Id} is not in participation team {Team.Id}");
        }

        public void AddFunction(FunctionEntity function)
        {
            ValidateFunction(function);
            if (_functions.Contains(function))
                throw new DomainException($"Function {function.Id} already in functions");
            _functions.Add(function);
            ReorderFunctions();
        }

        public void RemoveFunction(FunctionId id)
        {
            var stepToRemove = Functions.FirstOrDefault(f => f.Id == id);
            if (stepToRemove is null)
                throw new DomainException($"Function {id} not found in participation {Id}");
            _functions.Remove(stepToRemove);
            ReorderFunctions();
        }

        private void ReorderFunctions()
        {
            var orderedFunctions = _functions.ToList();
            for (var i = 0; i < orderedFunctions.Count; i++)
            {
                if (orderedFunctions[i].Order is not null)
                    orderedFunctions[i].Order = i;
            }

            _functions = new SortedSet<FunctionEntity>(orderedFunctions);
        }

        public void SetEndDate(DateTime date)
        {
            if (date <= StartDate)
                throw new DomainException("End date couldn't be lower or equal than start date");
            if (date <= EndDate)
                throw new DomainException("New end date couldn't be lower or equal than old end date");
            EndDate = date;
        }

        public void SetCalculatedScore(decimal calculatedScore)
        {
            if (calculatedScore < 0)
                throw new DomainException("Calculated can't be lesser than 0");
            CalculatedScore = calculatedScore;
        }

        public void Update(DateTime? endDate, decimal calculatedScore, IList<FunctionEntity> functions)
        {
            if (endDate.HasValue)
                SetEndDate(endDate.Value);
            SetCalculatedScore(calculatedScore);
            SetFunctions(functions);
        }
    }
}