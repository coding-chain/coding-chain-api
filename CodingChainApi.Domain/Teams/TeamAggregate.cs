using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Tournaments;
using Domain.Users;

namespace Domain.Teams
{
    public record TeamId(Guid Value) : IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class TeamAggregate : Aggregate<TeamId>
    {
        private readonly List<MemberEntity> _members;

        private readonly List<TournamentId> _tournamentIds;

        private TeamAggregate(TeamId id, string name, List<MemberEntity> members, List<TournamentId> tournamentIds) :
            base(id)
        {
            Name = name;
            _members = members;
            _tournamentIds = tournamentIds;
        }

        public IReadOnlyList<MemberEntity> Members => _members.AsReadOnly();
        private MemberEntity Admin => _members.First(u => u.IsAdmin);

        public IReadOnlyList<TournamentId> TournamentIds => _tournamentIds.AsReadOnly();
        public string Name { get; private set; }

        public static TeamAggregate CreateNew(TeamId id, string name, MemberEntity admin)
        {
            if (!admin.IsAdmin)
                throw new DomainException(new List<string> {"Cannot create team without admin member"});

            return new TeamAggregate(id, name, new List<MemberEntity> {admin}, new List<TournamentId>());
        }

        public static TeamAggregate Restore(TeamId id, string name, List<MemberEntity> members,
            List<TournamentId> tournamentIds)
        {
            return new(id, name, members, tournamentIds);
        }

        public void ValidateMemberAdditionByMember(UserId requestingUserId)
        {
            if (requestingUserId != Admin.Id)
                throw new DomainException($"User with id {requestingUserId} doesn't has sufficient rights to add user");
        }

        public void ValidateMemberDeletionByMember(UserId requestingUserId, UserId targetMemberId)
        {
            if (requestingUserId != Admin.Id && requestingUserId != targetMemberId)
                throw new DomainException(
                    $"User with id {requestingUserId} doesn't has sufficient rights to remove user");
        }

        public void ValidateMemberElevationByMember(UserId requestingUserId)
        {
            if (requestingUserId != Admin.Id)
                throw new DomainException(
                    $"User with id {requestingUserId} doesn't has sufficient rights to elevate user");
        }

        public void ValidateTeamRenamingByMember(UserId requestingUserId)
        {
            if (requestingUserId != Admin.Id)
                throw new DomainException(
                    $"User with id {requestingUserId} doesn't has sufficient rights to rename team");
        }

        public void Rename(string newName)
        {
            Name = newName;
        }

        public void AddMember(MemberEntity newMember)
        {
            if (_members.Contains(newMember))
                throw new DomainException(new List<string> {$"Member with id {newMember.Id} already in team"});

            if (newMember.IsAdmin) Admin.IsAdmin = false;

            _members.Add(newMember);
        }

        public void RemoveMember(UserId memberId)
        {
            var teamMember = GetMember(memberId);
            if (Admin.Id == teamMember.Id)
                throw new DomainException(new List<string>
                    {$"Member with id {teamMember.Id} cannot be removed because it's the team administrator"});

            _members.Remove(teamMember);
        }

        private MemberEntity GetMember(UserId memberId)
        {
            var teamMember = _members.FirstOrDefault(m => m.Id == memberId);
            if (teamMember is null)
                throw new DomainException(new List<string> {$"User with id {memberId} is not team member"});

            return teamMember;
        }

        public void ElevateMember(UserId memberId)
        {
            var teamMember = GetMember(memberId);
            Admin.IsAdmin = false;
            teamMember.IsAdmin = true;
        }

        public void ValidateTeamDeletionByMember(UserId memberId)
        {
            if (Admin.Id != memberId)
                throw new DomainException(new List<string> {$"Member with id {memberId} can't delete team"});
        }

        public void LeaveTournament(TournamentId tournamentId, UserId memberId)
        {
            ValidateTournamentLeaving(tournamentId, memberId);
            _tournamentIds.Remove(tournamentId);
        }

        public void ValidateTournamentLeaving(TournamentId tournamentId, UserId memberId)
        {
            var errors = new List<string>();
            if (Admin.Id != memberId)
                errors.Add($"Member with id {memberId} can't delete team");
            if (!_tournamentIds.Contains(tournamentId))
                errors.Add($"Team is not in tournament : {tournamentId}");
            if (errors.Any())
                throw new DomainException(errors);
        }
    }
}