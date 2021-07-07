using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Participations;
using Domain.StepEditions;
using Domain.Users;

namespace Domain.ParticipationSessions
{
    public record ConnectedUserUpdated(ParticipationId ParticipationId, UserId UserId) : IDomainEvent;

    public record ProcessResultUpdated(ParticipationId ParticipationId) : IDomainEvent;

    public record ProcessStarted(ParticipationId ParticipationId) : IDomainEvent;

    public record ParticipationSessionReady(ParticipationId ParticipationId) : IDomainEvent;

    public record ParticipationUserRemoved(ParticipationId ParticipationId, UserId UserId) : IDomainEvent;

    public record ParticipationSessionFunctionRemoved
        (ParticipationId ParticipationId, FunctionId FunctionId) : IDomainEvent;

    public record ParticipationSessionScoreChanged(ParticipationId ParticipationId) : IDomainEvent;

    public class ParticipationSessionAggregate : ParticipationAggregate
    {
        private List<TestId> _passedTestsIds = new();

        protected ParticipationSessionAggregate(ParticipationId id, TeamSessionEntity team,
            TournamentEntity tournamentEntity, StepSessionEntity stepSessionEntity, DateTime startDate,
            DateTime? endDate,
            decimal calculatedScore, IList<FunctionEntity> functions, bool isReady) : base(id, team, tournamentEntity,
            stepSessionEntity,
            startDate, endDate, calculatedScore, functions)
        {
            ConnectedTeam = team;
            StepSessionEntity = stepSessionEntity;
            IsReady = isReady;
        }

        public IReadOnlyCollection<TestId> PassedTestsIds => _passedTestsIds.AsReadOnly();
        public TeamSessionEntity ConnectedTeam { get; }
        public override TeamEntity Team => ConnectedTeam;
        public StepSessionEntity StepSessionEntity { get; }

        public override StepEntity StepEntity => StepSessionEntity;

        public string? LastError { get; private set; }
        public string? LastOutput { get; private set; }


        public bool IsReady { get; private set; }

        public DateTime? ProcessStartTime { get; private set; }

        public bool HasConnectedUsers => ConnectedTeam.ConnectedUserEntities.Any();


        public static ParticipationSessionAggregate Restore(
            ParticipationId id, TeamSessionEntity team, TournamentEntity tournament, StepSessionEntity step,
            DateTime startDate, DateTime? endDate, decimal calculatedScore, bool isReady,
            IList<FunctionEntity> functions)
        {
            return new(
                id,
                team,
                tournament,
                step,
                startDate,
                endDate,
                calculatedScore,
                functions,
                isReady);
        }

        public int AddConnectedUser(UserId userId)
        {
            if (!ConnectedTeam.UserIds.Contains(userId))
                throw new DomainException($"Can't add connected user {userId} isn't in participation team");

            var user = !ConnectedTeam.ConnectedUserEntities.Any()
                ? new ConnectedUserEntity(userId, true)
                : new ConnectedUserEntity(userId, false);
            var existingUser = ConnectedTeam.ConnectedUserEntities.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser is null)
            {
                user.ConnectionCount++;
                ConnectedTeam.ConnectedUserEntities.Add(user);
                return user.ConnectionCount;
            }

            existingUser.ConnectionCount++;
            return existingUser.ConnectionCount;
        }

        public override void RemoveFunction(FunctionId functionId, UserId userId)
        {
            if (!Team.UserIds.Contains(userId))
                throw new DomainException(
                    $"Function {functionId} cannot be deleted by user : {userId} not in participation team");

            RemoveFunction(functionId);
            RegisterEvent(new ParticipationSessionFunctionRemoved(Id, functionId));
        }

        public int RemoveConnectedUser(UserId userId)
        {
            if (!ConnectedTeam.UserIds.Contains(userId))
                throw new DomainException($"Can't disconnect user {userId} isn't in participation team");

            var existingUser = ConnectedTeam.ConnectedUserEntities.FirstOrDefault(u => u.Id == userId);
            if (existingUser is null) throw new DomainException($"Can't disconnect user {userId} isn't connected");

            existingUser.ConnectionCount--;
            if (existingUser.ConnectionCount == 0) ConnectedTeam.ConnectedUserEntities.Remove(existingUser);

            if (existingUser.IsAdmin)
            {
                var newAdmin = ConnectedTeam.ConnectedUserEntities.FirstOrDefault();
                if (newAdmin is not null)
                {
                    newAdmin.IsAdmin = true;
                    RegisterEvent(new ConnectedUserUpdated(Id, newAdmin.Id));
                }
            }

            return existingUser.ConnectionCount;
        }

        public void SetProcessResult(string? error, string? output, IList<TestId> testsPassedIds,
            DateTime participationEndTime)
        {
            LastError = error;
            LastOutput = output;
            var newScore = StepSessionEntity.Tests
                .Where(t => testsPassedIds.Contains(t.Id))
                .Sum(t => t.Score);
            SetCalculatedScore(newScore);
            _passedTestsIds = testsPassedIds.ToList();
            ProcessStartTime = null;
            if (_passedTestsIds.Count == StepSessionEntity.Tests.Count) SetEndDate(participationEndTime);
            RegisterEvent(new ProcessResultUpdated(Id));
        }

        public void ValidateProcessStart(UserId userId)
        {
            if (ProcessStartTime is not null)
                throw new DomainException($"Process already started at {ProcessStartTime.ToString()}");
            if (ConnectedTeam.TeamAdmin?.Id != userId)
                throw new DomainException($"User {userId} is not admin and he can't run code processing");
        }

        public void StartProcess(DateTime startTime)
        {
            _passedTestsIds = new List<TestId>();
            ProcessStartTime = startTime;
            RegisterEvent(new ProcessStarted(Id));
        }

        public void SetReadyState()
        {
            if (IsReady)
                throw new DomainException($"Participation {Id} is already ready");
            IsReady = true;
            RegisterEvent(new ParticipationSessionReady(Id));
        }


        public void ElevateUser(UserId elevationTarget, UserId currentUser)
        {
            if (ConnectedTeam.TeamAdmin?.Id != currentUser)
                throw new DomainException(
                    $"User {currentUser} is not admin and he can't elevate to admin user {elevationTarget}");
            var user = ConnectedTeam.ConnectedUserEntities.FirstOrDefault(u => u.Id == elevationTarget);
            if (user is null)
                throw new DomainException($"User {elevationTarget} is not connected");
            ConnectedTeam.TeamAdmin.IsAdmin = false;
            user.IsAdmin = true;
            RegisterEvent(new ConnectedUserUpdated(Id, elevationTarget));
            RegisterEvent(new ConnectedUserUpdated(Id, currentUser));
        }

        public override void RemoveSuspectFunction(FunctionId functionId)
        {
            RemoveFunction(functionId);
            RegisterEvent(new ParticipationSessionFunctionRemoved(Id, functionId));
            SetCalculatedScore(0);
        }

        public override void SetCalculatedScore(decimal calculatedScore)
        {
            if (calculatedScore < 0)
                throw new DomainException("Calculated can't be lesser than 0");
            CalculatedScore = calculatedScore;
            RegisterEvent(new ParticipationSessionScoreChanged(Id));
        }

        public void AddTeamMember(UserId userId)
        {
            if (Team.UserIds.Any(id => id == userId))
            {
                throw new DomainException("User already in participation team");
            }

            Team.UserIds.Add(userId);
        }

        public void RemoveTeamMember(UserId userId)
        {
            var user = Team.UserIds.FirstOrDefault(id => id == userId);
            if (user is null)
            {
                throw new DomainException("User not in participation team");
            }

            Team.UserIds.Remove(user);
            ConnectedTeam.ConnectedUserEntities.RemoveWhere(u => u.Id == userId);
            RegisterEvent(new ParticipationUserRemoved(Id, userId));
        }
    }
}