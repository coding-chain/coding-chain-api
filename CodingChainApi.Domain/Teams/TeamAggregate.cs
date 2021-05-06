using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;
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

    public class MemberEntity : Entity<UserId>
    {
        public bool IsAdmin { get; set; }

        public MemberEntity(UserId id, bool isAdmin) : base(id)
        {
            IsAdmin = isAdmin;
        }
    }

    public class TeamAggregate : Aggregate<TeamId>
    {
        public IReadOnlyList<MemberEntity> Members => _members.AsReadOnly();
        private List<MemberEntity> _members;
        private MemberEntity Admin => _members.First(u => u.IsAdmin);
        public string Name { get; private set; }

        public TeamAggregate(TeamId id, string name, List<MemberEntity> members) : base(id)
        {
            Name = name;
            if (members.All(m => !m.IsAdmin))
            {
                throw new DomainException(new List<string>() {"Cannot create team without admin member"});
            }

            _members = members;
        }

        public void ValidateMemberAdditionByMember(UserId requestingUserId)
        {
            if (requestingUserId != Admin.Id)
                throw new DomainException($"User with id {requestingUserId} doesn't has sufficient rights to add user");
        }
        
        public void ValidateMemberDeletionByMember(UserId requestingUserId, UserId targetMemberId)
        {
            if (requestingUserId != Admin.Id && requestingUserId != targetMemberId)
                throw new DomainException($"User with id {requestingUserId} doesn't has sufficient rights to remove user");
        }
        
        public void ValidateMemberElevationByMember(UserId requestingUserId)
        {
            if (requestingUserId != Admin.Id )
                throw new DomainException($"User with id {requestingUserId} doesn't has sufficient rights to elevate user");
        }
        
        public void ValidateTeamRenamingByMember(UserId requestingUserId)
        {
            if (requestingUserId != Admin.Id )
                throw new DomainException($"User with id {requestingUserId} doesn't has sufficient rights to rename team");
        }

        public void Rename(string newName)
        {
            Name = newName;
        }

        public void AddMember(MemberEntity newMember)
        {
            if (_members.Any(m => m.Id == newMember.Id))
            {
                throw new DomainException(new List<string>() {$"Member with id {newMember.Id} already in team"});
            }

            if (newMember.IsAdmin)
            {
                Admin.IsAdmin = false;
            }

            _members.Add(newMember);
        }

        public void RemoveMember(UserId memberId)
        {
            var teamMember = GetMember(memberId);
            if (Admin.Id == teamMember.Id)
            {
                throw new DomainException(new List<string>()
                    {$"Member with id {teamMember.Id} cannot be removed because it's the team administrator"});
            }

            _members.Remove(teamMember);
        }

        private MemberEntity GetMember(UserId memberId)
        {
            var teamMember = _members.FirstOrDefault(m => m.Id == memberId);
            if (teamMember is null)
            {
                throw new DomainException(new List<string>() {$"User with id {memberId} is not team member"});
            }

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
            {
                throw new DomainException(new List<string>() {$"Member with id {memberId} can't delete team"});
            }
        }
    }
}