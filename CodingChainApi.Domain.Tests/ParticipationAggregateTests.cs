using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;
using Domain.Participations;
using Domain.StepEditions;
using Domain.Teams;
using Domain.Tournaments;
using Domain.Users;
using NUnit.Framework;
using StepEntity = Domain.Participations.StepEntity;

namespace CodingChainApi.Domain.Tests
{
    public class ParticipationAggregateTests
    {
        private FunctionId GetNewFunctionId() => new FunctionId(Guid.NewGuid());
        private ParticipationId GetNewParticipationId() => new ParticipationId(Guid.NewGuid());
        private TournamentId GetNewTournamentId() => new TournamentId(Guid.NewGuid());
        private TeamId GetNewTeamId() => new TeamId(Guid.NewGuid());
        private StepId GetNewStepId() => new StepId(Guid.NewGuid());
        private UserId GetNewUserId() => new UserId(Guid.NewGuid());

        private UserId _teamUser;

        private IList<FunctionEntity> GetValidFunctions(UserId userId) => new List<FunctionEntity>()
        {
            new(GetNewFunctionId(), userId, "", DateTime.Now, 0),
            new(GetNewFunctionId(), userId, "", DateTime.Now.AddMinutes(2), 1),
            new(GetNewFunctionId(), userId, "", DateTime.Now.AddMinutes(3), null)
        };

        private TeamEntity GetValidTeam(IList<UserId> userIds = null) => new(GetNewTeamId(),
            userIds ?? new List<UserId>() {_teamUser, GetNewUserId()});

        private StepEntity GetValidStep(TournamentId tournamentId) =>
            new(GetNewStepId(), new List<TournamentId>() {tournamentId, GetNewTournamentId()});

        private TournamentEntity GetPublishedTournament() => new(GetNewTournamentId(), true);
        private TournamentEntity GetNotPublishedTournament() => new(GetNewTournamentId(), false);

        private ParticipationAggregate GetValidParticipation(DateTime? endDate = null)
        {
            var tournament = GetPublishedTournament();
            var step = GetValidStep(tournament.Id);
            var team = GetValidTeam();
            var functions = GetValidFunctions(team.UserIds.First());
            return ParticipationAggregate.Restore(
                GetNewParticipationId(), team, tournament, step,
                endDate?.AddDays(-3) ?? DateTime.Now, endDate, 0, functions);
        }

        [SetUp]
        public void Setup()
        {
            _teamUser = GetNewUserId();
        }

        [Test]
        public void set_calculated_score_bellow_0_should_throw()
        {
            var participation = GetValidParticipation();
            Assert.Throws<DomainException>(() => participation.SetCalculatedScore(-3));
        }

        [Test]
        public void set_calculated_score_should_work()
        {
            var participation = GetValidParticipation();
            const int expectedScore = 3;
            participation.SetCalculatedScore(expectedScore);
            Assert.AreEqual(expectedScore, participation.CalculatedScore);
        }

        [Test]
        public void set_end_date_bellow_start_date_should_throw()
        {
            var participation = GetValidParticipation();
            Assert.Throws<DomainException>(() => participation.SetEndDate(participation.StartDate.AddDays(-2)));
        }

        [Test]
        public void set_end_date_bellow_current_end_date_should_throw()
        {
            var currentEndDate = DateTime.Now;
            var participation = GetValidParticipation(currentEndDate);
            Assert.Throws<DomainException>(() => participation.SetEndDate(currentEndDate.AddDays(-1)));
        }

        [Test]
        public void set_end_date_should_work()
        {
            var participation = GetValidParticipation();
            var expectedEndDate = participation.StartDate.AddDays(3);
            participation.SetEndDate(expectedEndDate);
            Assert.AreEqual(expectedEndDate, participation.EndDate);
        }

        [Test]
        public void add_function_should_throw_if_function_already_contained()
        {
            var participation = GetValidParticipation();
            Assert.Throws<DomainException>(() =>
                participation.AddFunction(participation.Functions.First(), participation.Team.UserIds.First()));
        }

        [Test]
        public void add_function_should_throw_if_function_user_is_not_in_team()
        {
            var participation = GetValidParticipation();
            var newFunction = new FunctionEntity(GetNewFunctionId(), GetNewUserId(), "", DateTime.Now, null);
            Assert.Throws<DomainException>(() => participation.AddFunction(newFunction, _teamUser));
        }

        [Test]
        public void add_function_should_work()
        {
            var participation = GetValidParticipation();
            var userId = participation.Team.UserIds.First();
            var expectedFunction = new FunctionEntity(GetNewFunctionId(), userId, "", DateTime.Now, null);
            participation.AddFunction(expectedFunction, _teamUser);
            CollectionAssert.Contains(participation.Functions, expectedFunction);
        }

        [Test]
        public void remove_function_should_throw_if_function_is_not_found()
        {
            var participation = GetValidParticipation();
            Assert.Throws<DomainException>(() => participation.RemoveFunction(GetNewFunctionId(), _teamUser));
        }


        [Test]
        public void remove_function_should_work()
        {
            var participation = GetValidParticipation();
            var function = participation.Functions.First();
            participation.RemoveFunction(function.Id, _teamUser);
            CollectionAssert.DoesNotContain(participation.Functions, function);
        }

        [Test]
        public void set_functions_should_throw_on_unordered_functions()
        {
            var participation = GetValidParticipation();
            var validUserId = participation.Team.UserIds.First();
            var unorderedFunctions = new List<FunctionEntity>()
            {
                new(GetNewFunctionId(), validUserId, "", DateTime.Now, 1),
                new(GetNewFunctionId(), validUserId, "", DateTime.Now, 4)
            };
            Assert.Throws<DomainException>(() => participation.SetFunctions(unorderedFunctions));
        }

        [Test]
        public void set_functions_should_work_on_ordered_functions()
        {
            var participation = GetValidParticipation();
            var validUserId = participation.Team.UserIds.First();
            var expectedFunctions = new List<FunctionEntity>()
            {
                new(GetNewFunctionId(), validUserId, "", DateTime.Now, 1),
                new(GetNewFunctionId(), validUserId, "", DateTime.Now, null),
                new(GetNewFunctionId(), validUserId, "", DateTime.Now, 2)
            };
            participation.SetFunctions(expectedFunctions);
            CollectionAssert.AreEquivalent(participation.Functions, expectedFunctions);
        }

        [Test]
        public void set_functions_should_throw_on_not_in_team_user()
        {
            var participation = GetValidParticipation();
            var validUserId = participation.Team.UserIds.First();
            var notFoundUserFunctions = new List<FunctionEntity>()
            {
                new(GetNewFunctionId(), GetNewUserId(), "", DateTime.Now, 1),
                new(GetNewFunctionId(), validUserId, "", DateTime.Now, 2)
            };
            Assert.Throws<DomainException>(() => participation.SetFunctions(notFoundUserFunctions));
        }

        [Test]
        public void set_functions_should_throw_on_function_with_last_modification_date_lower_than_its_previous_version()
        {
            var participation = GetValidParticipation();
            var validFunction = participation.Functions.First();
            var failingFunction = new FunctionEntity(validFunction.Id, validFunction.UserId, validFunction.Code,
                validFunction.LastModificationDate.AddDays(-1), null);
            Assert.Throws<DomainException>(() =>
                participation.SetFunctions(new List<FunctionEntity> {failingFunction}));
        }
    }
}