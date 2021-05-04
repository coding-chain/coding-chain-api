using System;
using System.Collections.Generic;
using Domain.Exceptions;
using Domain.Teams;
using Domain.Users;
using NUnit.Framework;

namespace CodingChainApi.Domain.Tests
{
    public class TeamAggregateTests
    {
        private string _teamName;
        private TeamId _teamId;
        private MemberEntity _adminMember;

        [SetUp]
        public void Setup()
        {
            _teamName = "TestTeam";
            _teamId = new TeamId(Guid.NewGuid());
            _adminMember = new MemberEntity(new UserId(Guid.NewGuid()), true);
        }

        private MemberEntity GetCommonMember() => new(new UserId(Guid.NewGuid()), false);

        [Test]
        public void create_team_without_admin_should_throw()
        {
            Assert.Throws<DomainException>(() =>
            {
                new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {GetCommonMember()});
            });
        }

        [Test]
        public void create_team_should_work()
        {
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember});
            Assert.AreEqual(_teamId, team.Id);
        }

        [Test]
        public void validate_member_deletion_should_throw_if_not_admin_and_not_same_user()
        {
            var noRightsMember = GetCommonMember();
            var targetedMember = GetCommonMember();
            var team = new TeamAggregate(_teamId, _teamName,
                new List<MemberEntity>() {_adminMember, noRightsMember, targetedMember});
            Assert.Throws<DomainException>(() =>
            {
                team.ValidateMemberDeletionByMember(noRightsMember.Id, targetedMember.Id);
            });
        }

        [Test]
        public void validate_member_deletion_should_work_if_admin()
        {
            var targetedMember = GetCommonMember();
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember, targetedMember});
            team.ValidateMemberDeletionByMember(_adminMember.Id, targetedMember.Id);
        }

        [Test]
        public void validate_member_deletion_should_work_if_same_user()
        {
            var targetedMember = GetCommonMember();
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember, targetedMember});
            team.ValidateMemberDeletionByMember(targetedMember.Id, targetedMember.Id);
        }

        [Test]
        public void remove_member_should_work()
        {
            var targetedMember = GetCommonMember();
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember, targetedMember});
            team.RemoveMember(targetedMember.Id);
            Assert.AreEqual(1, team.Members.Count);
            CollectionAssert.DoesNotContain(team.Members, targetedMember);
        }

        [Test]
        public void remove_member_should_throw_if_user_is_admin()
        {
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember});
            Assert.Throws<DomainException>(() => team.RemoveMember(_adminMember.Id));
        }
        
        [Test]
        public void remove_member_should_throw_if_user_not_found()
        {
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember});
            Assert.Throws<DomainException>(() => team.RemoveMember(GetCommonMember().Id));
        }

        [Test]
        public void validate_member_addition_should_throw_if_not_admin()
        {
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember});
            Assert.Throws<DomainException>(() => team.ValidateMemberAdditionByMember(GetCommonMember().Id));
        }

        [Test]
        public void validate_member_addition_should_work_if_admin()
        {
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember});
            team.ValidateMemberAdditionByMember(_adminMember.Id);
        }

        [Test]
        public void add_member_should_throw_if_user_already_exists()
        {
            var existingUser = GetCommonMember();
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember, existingUser});
            Assert.Throws<DomainException>(() => team.AddMember(existingUser));
        }
        
        [Test]
        public void add_member_should_works()
        {
            var newMember = GetCommonMember();
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember});
            team.AddMember(newMember);
            Assert.AreEqual(2, team.Members.Count);
            CollectionAssert.Contains(team.Members, newMember);
        }
        [Test]
        public void elevate_member_should_throw_if_not_admin()
        {
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember});
            Assert.Throws<DomainException>(() => team.ValidateMemberElevationByMember(GetCommonMember().Id));
        }
        [Test]
        public void elevate_member_should_throw_if_not_exists()
        {
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember});
            Assert.Throws<DomainException>(() => team.ElevateMember(GetCommonMember().Id));
        }
        [Test]
        public void elevate_member_should_work()
        {
            var targetMember = GetCommonMember();
            var team = new TeamAggregate(_teamId, _teamName, new List<MemberEntity>() {_adminMember,targetMember});
            team.ElevateMember(targetMember.Id);
            Assert.AreEqual(true,targetMember.IsAdmin);
            Assert.AreEqual(false,_adminMember.IsAdmin);
        }
    }
}