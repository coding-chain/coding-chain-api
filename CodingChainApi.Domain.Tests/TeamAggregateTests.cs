using System;
using System.Collections.Generic;
using Domain.Exceptions;
using Domain.Teams;
using Domain.Tournaments;
using Domain.Users;
using NUnit.Framework;

namespace CodingChainApi.Domain.Tests
{
    public class TeamAggregateTests
    {
        private MemberEntity _adminMember;
        private TeamId _teamId;
        private string _teamName;

        [SetUp]
        public void Setup()
        {
            _teamName = "TestTeam";
            _teamId = new TeamId(Guid.NewGuid());
            _adminMember = new MemberEntity(new UserId(Guid.NewGuid()), true);
        }

        private MemberEntity GetCommonMember()
        {
            return new(new UserId(Guid.NewGuid()), false);
        }



        private TeamAggregate GetValidTeam()
        {
            return TeamAggregate.Restore(_teamId, _teamName, new List<MemberEntity> {_adminMember},
                new List<TournamentId>());
        }


        [Test]
        public void create_team_without_admin_should_throw()
        {
            Assert.Throws<DomainException>(() => { TeamAggregate.CreateNew(_teamId, _teamName, GetCommonMember()); });
        }

        [Test]
        public void create_team_should_work()
        {
            var team = GetValidTeam();
            Assert.AreEqual(_teamId, team.Id);
        }

        [Test]
        public void validate_member_deletion_should_throw_if_not_admin_and_not_same_user()
        {
            var noRightsMember = GetCommonMember();
            var targetedMember = GetCommonMember();
            var team = TeamAggregate.Restore(_teamId, _teamName,
                new List<MemberEntity> {_adminMember, noRightsMember, targetedMember}, new List<TournamentId>());
            Assert.Throws<DomainException>(() =>
            {
                team.ValidateMemberDeletionByMember(noRightsMember.Id, targetedMember.Id);
            });
        }

        [Test]
        public void validate_member_deletion_should_work_if_admin()
        {
            var targetedMember = GetCommonMember();
            var team = TeamAggregate.Restore(_teamId, _teamName,
                new List<MemberEntity> {_adminMember, targetedMember}, new List<TournamentId>());
            team.ValidateMemberDeletionByMember(_adminMember.Id, targetedMember.Id);
        }

        [Test]
        public void validate_member_deletion_should_work_if_same_user()
        {
            var targetedMember = GetCommonMember();
            var team = TeamAggregate.Restore(_teamId, _teamName,
                new List<MemberEntity> {_adminMember, targetedMember}, new List<TournamentId>());
            team.ValidateMemberDeletionByMember(targetedMember.Id, targetedMember.Id);
        }

        [Test]
        public void remove_member_should_work()
        {
            var targetedMember = GetCommonMember();
            var team = TeamAggregate.Restore(_teamId, _teamName,
                new List<MemberEntity> {_adminMember, targetedMember}, new List<TournamentId>());
            team.RemoveMember(targetedMember.Id);
            Assert.AreEqual(1, team.Members.Count);
            CollectionAssert.DoesNotContain(team.Members, targetedMember);
        }

        [Test]
        public void remove_member_should_throw_if_user_is_admin()
        {
            var team = GetValidTeam();
            Assert.Throws<DomainException>(() => team.RemoveMember(_adminMember.Id));
        }

        [Test]
        public void remove_member_should_throw_if_user_not_found()
        {
            var team = GetValidTeam();
            Assert.Throws<DomainException>(() => team.RemoveMember(GetCommonMember().Id));
        }

        [Test]
        public void validate_member_addition_should_throw_if_not_admin()
        {
            var team = GetValidTeam();
            Assert.Throws<DomainException>(() => team.ValidateMemberAdditionByMember(GetCommonMember().Id));
        }

        [Test]
        public void validate_team_rename_should_work_if_admin()
        {
            var team = GetValidTeam();
            team.ValidateTeamRenamingByMember(_adminMember.Id);
        }

        [Test]
        public void validate_team_rename_should_throw_if_not_admin()
        {
            var team = GetValidTeam();
            Assert.Throws<DomainException>(() => team.ValidateTeamRenamingByMember(GetCommonMember().Id));
        }

        [Test]
        public void rename_should_rename_team()
        {
            var newName = "New name";
            var team = GetValidTeam();
            var oldName = team.Name;
            team.Rename(newName);
            Assert.AreNotEqual(oldName, team.Name);
            Assert.AreEqual(newName, team.Name);
        }

        [Test]
        public void validate_member_addition_should_work_if_admin()
        {
            var team = GetValidTeam();
            team.ValidateMemberAdditionByMember(_adminMember.Id);
        }

        [Test]
        public void add_member_should_throw_if_user_already_exists()
        {
            var existingUser = GetCommonMember();
            var team = TeamAggregate.Restore(_teamId, _teamName, new List<MemberEntity> {_adminMember, existingUser},
                new List<TournamentId>());
            Assert.Throws<DomainException>(() => team.AddMember(existingUser));
        }

        [Test]
        public void add_member_should_works()
        {
            var newMember = GetCommonMember();
            var team = GetValidTeam();
            team.AddMember(newMember);
            Assert.AreEqual(2, team.Members.Count);
            CollectionAssert.Contains(team.Members, newMember);
        }

        [Test]
        public void elevate_member_should_throw_if_not_admin()
        {
            var team = GetValidTeam();
            Assert.Throws<DomainException>(() => team.ValidateMemberElevationByMember(GetCommonMember().Id));
        }

        [Test]
        public void elevate_member_should_throw_if_not_exists()
        {
            var team = GetValidTeam();
            Assert.Throws<DomainException>(() => team.ElevateMember(GetCommonMember().Id));
        }

        [Test]
        public void elevate_member_should_work()
        {
            var targetMember = GetCommonMember();
            var team = TeamAggregate.Restore(_teamId, _teamName, new List<MemberEntity> {_adminMember, targetMember},
                new List<TournamentId>());
            team.ElevateMember(targetMember.Id);
            Assert.AreEqual(true, targetMember.IsAdmin);
            Assert.AreEqual(false, _adminMember.IsAdmin);
        }

        [Test]
        public void leave_not_joined_tournament_should_throw()
        {
            var team = TeamAggregate.Restore(_teamId, _teamName, new List<MemberEntity> {_adminMember},
                new List<TournamentId> {TestsHelper.GetTournamentId()});
            Assert.Throws<DomainException>(() => team.LeaveTournament(TestsHelper.GetTournamentId(), _adminMember.Id));
        }

        [Test]
        public void leave_tournament_without_admin_account_should_throw()
        {
            var commonMember = GetCommonMember();
            var existingTournamentId = TestsHelper.GetTournamentId();
            var team = TeamAggregate.Restore(_teamId, _teamName, new List<MemberEntity> {_adminMember, commonMember},
                new List<TournamentId> {existingTournamentId});
            Assert.Throws<DomainException>(() => team.LeaveTournament(existingTournamentId, commonMember.Id));
        }

        [Test]
        public void leave_tournament_should_work()
        {
            var existingTournamentId = TestsHelper.GetTournamentId();
            var team = TeamAggregate.Restore(_teamId, _teamName, new List<MemberEntity> {_adminMember},
                new List<TournamentId> {existingTournamentId});
            team.LeaveTournament(existingTournamentId, _adminMember.Id);
            CollectionAssert.DoesNotContain(team.TournamentIds, existingTournamentId);
        }
    }
}