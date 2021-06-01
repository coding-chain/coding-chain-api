using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Participations;
using Domain.Users;

namespace Domain.ParticipationStates
{
    public record ConnectedUserAdded(ParticipationId ParticipationId, UserId UserId) : IDomainEvent;

    public record ConnectedUserRemoved(ParticipationId ParticipationId, UserId UserId) : IDomainEvent;

    public record ConnectedUserUpdated(ParticipationId ParticipationId, UserId UserId) : IDomainEvent;

    public record ProcessResultUpdated(ParticipationId ParticipationId) : IDomainEvent;

    public record ProcessStarted(ParticipationId ParticipationId) : IDomainEvent;

    public class ParticipationSessionAggregate : ParticipationAggregate
    {
        public TeamStateEntity ConnectedTeam { get; private init; }
        public override TeamEntity Team => ConnectedTeam;

        public string? LastError { get; private set; }
        public string? LastOutput { get; private set; }

        public DateTime? ProcessStartTime { get; private set; }

        public static ParticipationSessionAggregate FromParticipationAggregate(ParticipationAggregate participation)
        {
            var team = new TeamStateEntity(participation.Team.Id, participation.Team.UserIds,
                new List<ConnectedUserEntity>());
            return new ParticipationSessionAggregate(participation.Id, team, participation.TournamentEntity,
                participation.StepEntity,
                participation.StartDate, participation.EndDate, participation.CalculatedScore,
                participation.Functions.ToList());
        }

        protected ParticipationSessionAggregate(ParticipationId id, TeamStateEntity team,
            TournamentEntity tournamentEntity, StepEntity stepEntity, DateTime startDate, DateTime? endDate,
            decimal calculatedScore, IList<FunctionEntity> functions) : base(id, team, tournamentEntity, stepEntity,
            startDate, endDate, calculatedScore, functions)
        {
            ConnectedTeam = team;
        }

        public int AddConnectedUser(UserId userId)
        {
            if (!ConnectedTeam.UserIds.Contains(userId))
            {
                throw new DomainException($"Can't add connected user {userId} isn't in participation team");
            }

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

        public int RemoveConnectedUser(UserId userId)
        {
            if (!ConnectedTeam.UserIds.Contains(userId))
            {
                throw new DomainException($"Can't disconnect user {userId} isn't in participation team");
            }

            var existingUser = ConnectedTeam.ConnectedUserEntities.FirstOrDefault(u => u.Id == userId);
            if (existingUser is null)
            {
                throw new DomainException($"Can't disconnect user {userId} isn't connected");
            }

            existingUser.ConnectionCount--;
            if (existingUser.ConnectionCount == 0)
            {
                ConnectedTeam.ConnectedUserEntities.Remove(existingUser);
            }

            if (existingUser.IsAdmin)
            {
                var newAdmin = ConnectedTeam.ConnectedUserEntities.FirstOrDefault();
                if (newAdmin is not null)
                {
                    newAdmin.IsAdmin = true;
                    RegisterEvent(new ConnectedUserUpdated(this.Id, newAdmin.Id));
                }
            }

            return existingUser.ConnectionCount;
        }

        public void SetProcessResult(string? error, string? output)
        {
            LastError = error;
            LastOutput = output;
            ProcessStartTime = null;
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
            ProcessStartTime = startTime;
            RegisterEvent(new ProcessStarted(Id));
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
            RegisterEvent(new ConnectedUserUpdated(this.Id, elevationTarget));
            RegisterEvent(new ConnectedUserUpdated(this.Id, currentUser));
        }

    }
}