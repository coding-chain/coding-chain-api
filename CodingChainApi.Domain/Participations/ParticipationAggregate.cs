using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Users;

namespace Domain.Participations
{
    public record ParticipationFunctionAdded(ParticipationId ParticipationId, FunctionId FunctionId) : IDomainEvent;

    public record ParticipationFunctionUpdated(ParticipationId ParticipationId, FunctionId FunctionId) : IDomainEvent;

    public record ParticipationFunctionsReordered
        (ParticipationId ParticipationId, IList<FunctionId> FunctionIds) : IDomainEvent;

    public record ParticipationFunctionRemoved(ParticipationId ParticipationId, FunctionId FunctionId) : IDomainEvent;

    public record ParticipationId(Guid Value) : IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class ParticipationAggregate : Aggregate<ParticipationId>
    {
        public virtual TeamEntity Team { get; private set; }
        public TournamentEntity TournamentEntity { get; private set; }
        public virtual StepEntity StepEntity { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public decimal CalculatedScore { get; private set; }
        public IReadOnlyList<FunctionEntity> Functions => _functions.ToList().AsReadOnly();
        private List<FunctionEntity> _functions;


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

        protected ParticipationAggregate(ParticipationId id, TeamEntity team, TournamentEntity tournamentEntity,
            StepEntity stepEntity,
            DateTime startDate, DateTime? endDate, decimal calculatedScore, IList<FunctionEntity> functions) : base(id)
        {
            Team = team;
            TournamentEntity = tournamentEntity;
            StepEntity = stepEntity;
            _functions = new List<FunctionEntity>(functions);
            StartDate = startDate;
            EndDate = endDate;
            CalculatedScore = calculatedScore;
        }

        public void SetFunctions(IList<FunctionEntity> functions)
        {
            var orderedFunctions = new List<FunctionEntity>(functions);
            orderedFunctions.Sort((f1,f2) => (f1.Order ?? 0) - (f2.Order ?? 0));
            var i = 1;
            var errors = orderedFunctions
                .SelectMany((function) =>
                {
                    var subErrors = new List<string>();
                    if (function.Order is not null)
                    {
                        if (function.Order != i)
                            subErrors.Add($"Function {function.Id} and order {function.Order} isn't ordered");
                        i++;
                    }
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

        public void AddFunction(FunctionEntity function, UserId userId)
        {
            if (!Team.UserIds.Contains(userId))
            {
                throw new DomainException(
                    $"Function {function.Id} cannot be added by user : {userId} not in participation team");
            }

            ValidateFunction(function);
            if (_functions.ToList().Any(f => f.Id == function.Id))
                throw new DomainException($"Function {function.Id} already in functions");
            _functions.Add(function);
            ReorderFunctions();
            RegisterEvent(new ParticipationFunctionAdded(Id, function.Id));
        }

        public void UpdateFunction(FunctionEntity function, UserId userId)
        {
            if (!Team.UserIds.Contains(userId))
            {
                throw new DomainException(
                    $"Function {function.Id} cannot be added by user : {userId} not in participation team");
            }

            ValidateFunction(function);
            var existingFunction = _functions.FirstOrDefault(f => f.Id == function.Id);
            if (existingFunction is null)
                throw new DomainException($"Function {function.Id} not in participation functions");
            var otherFunctionWithSameOrder =
                _functions.FirstOrDefault(f => f.Order == function.Order && function.Order != null);
            if (otherFunctionWithSameOrder is not null && existingFunction.Order is not null)
            {
                otherFunctionWithSameOrder.Order = existingFunction.Order;
                RegisterEvent(new ParticipationFunctionsReordered(this.Id,
                    new List<FunctionId>() {otherFunctionWithSameOrder.Id, function.Id}));
            }

            if (existingFunction.Order is null && function.Order is not null)
            {
                ShiftExistingFunctionsOrder(function.Order.Value);
            } 
            existingFunction.Order = function.Order;
            existingFunction.Code = function.Code;
            existingFunction.LastModificationDate = function.LastModificationDate;
            existingFunction.UserId = function.UserId;
            ReorderFunctions();
            RegisterEvent(new ParticipationFunctionUpdated(Id, function.Id));
        }


        public void RemoveFunction(FunctionId functionId, UserId userId)
        {
            if (!Team.UserIds.Contains(userId))
            {
                throw new DomainException(
                    $"Function {functionId} cannot be deleted by user : {userId} not in participation team");
            }

            var stepToRemove = Functions.FirstOrDefault(f => f.Id == functionId);
            if (stepToRemove is null)
                throw new DomainException($"Function {functionId} not found in participation {Id}");
            _functions.Remove(stepToRemove);
            ReorderFunctions();
            RegisterEvent(new ParticipationFunctionRemoved(Id, functionId));
        }

        private void ShiftExistingFunctionsOrder(int order)
        {
            var reorderedFunctions = new List<FunctionId>();
            GetSortedFunctions().ForEach(f =>
            {
                if (f.Order >= order)
                {
                    f.Order++;
                    reorderedFunctions.Add(f.Id);
                }
            });
            if (reorderedFunctions.Any())
            {
                RegisterEvent(new ParticipationFunctionsReordered(this.Id, reorderedFunctions));
            }
        }

        private IList<FunctionId> ReorderFunctions()
        {
            var functionsWithOrder = GetSortedFunctions();
            var functionsWithDifferentIds = new List<FunctionId>();
            for (var i = 0; i < functionsWithOrder.Count; i++)
            {
                if (functionsWithOrder[i].Order == i + 1) continue;
                functionsWithDifferentIds.Add(functionsWithOrder[i].Id);
                functionsWithOrder[i].Order = i + 1;
            }

            if (functionsWithDifferentIds.Any())
            {
                RegisterEvent(new ParticipationFunctionsReordered(this.Id, functionsWithDifferentIds));
            }
            return functionsWithDifferentIds;
        }

        private List<FunctionEntity> GetSortedFunctions()
        {
            var functionsWithOrder = _functions
                .ToList()
                .Where(f => f.Order is not null)
                .ToList();
            functionsWithOrder.Sort((f1, f2) => f1.Order.Value - f2.Order.Value);
            return functionsWithOrder;
        }

        public void SetEndDate(DateTime? date)
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