using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;
using Domain.StepEditions;
using Domain.Teams;
using Domain.Tournaments;
using Domain.Users;
using NUnit.Framework;

namespace CodingChainApi.Domain.Tests
{
    public class TournamentAggregateTests
    {
        private string _tournamentName;
        private string _tournamentDescription;
        private TournamentId _tournamentId;

        [SetUp]
        public void Setup()
        {
            _tournamentName = "Test tournament name";
            _tournamentDescription = "Test tournament description";
            _tournamentId = new TournamentId(Guid.NewGuid());
        }

        private StepEntity GetStep(int order, bool isOptional) =>
            new StepEntity(new StepId(Guid.NewGuid()), order, isOptional);

        private TournamentAggregate GetNewTournament() => TournamentAggregate.CreateNew(
            _tournamentId,
            _tournamentName,
            _tournamentDescription);

        private TournamentAggregate GetNewTournamentWithSteps() => TournamentAggregate.Restore(
            _tournamentId,
            _tournamentName,
            _tournamentDescription,
            false,
            null,
            null,
            new List<StepEntity>
            {
                GetStep(0, false),
                GetStep(1, false)
            });

        private TournamentAggregate GetPublishedTournament(DateTime? startDate = null, DateTime? endDate = null) =>
            TournamentAggregate.Restore(
                _tournamentId,
                _tournamentName,
                _tournamentDescription,
                true,
                startDate ?? DateTime.Now,
                endDate ?? DateTime.Now.AddDays(2),
                new List<StepEntity>
                {
                    GetStep(0, false),
                    GetStep(1, false)
                });

        [Test]
        public void create_new_tournament_should_work()
        {
            var tournament = GetNewTournament();
            Assert.AreEqual(_tournamentId, tournament.Id);
            Assert.AreEqual(_tournamentName, tournament.Name);
            Assert.AreEqual(_tournamentDescription, tournament.Description);
            Assert.Null(tournament.StartDate);
            Assert.Null(tournament.EndDate);
            Assert.IsEmpty(tournament.Steps);
        }

        [Test]
        public void set_end_date_before_start_date_should_throw()
        {
            var tournament = GetNewTournament();
            tournament.SetStartDate(DateTime.Now);
            Assert.Throws<DomainException>(() => tournament.SetEndDate(DateTime.Now.AddDays(-1)));
        }

        [Test]
        public void set_end_date_on_published_tournament_should_throw()
        {
            var tournament = GetPublishedTournament();
            Assert.Throws<DomainException>(() => tournament.SetEndDate(DateTime.Now));
        }


        [Test]
        public void set_end_date_equals_to_start_date_should_throw()
        {
            var startDate = DateTime.Now;
            var tournament = GetNewTournament();
            tournament.SetStartDate(startDate);
            Assert.Throws<DomainException>(() => tournament.SetEndDate(startDate));
        }

        [Test]
        public void set_end_date_without_start_date_should_throw()
        {
            var tournament = GetNewTournament();
            Assert.Throws<DomainException>(() => tournament.SetEndDate(DateTime.Now));
        }

        [Test]
        public void set_end_date_should_works()
        {
            var startDate = DateTime.Now;
            var tournament = GetNewTournament();
            tournament.SetStartDate(startDate);
            var endDate = startDate.AddDays(1);
            tournament.SetEndDate(endDate);
            Assert.AreEqual(endDate, tournament.EndDate);
        }

        [Test]
        public void set_start_date_greater_than_end_date_should_throw()
        {
            var endDate = DateTime.Now;
            var tournament = TournamentAggregate.Restore(
                _tournamentId, _tournamentName, _tournamentDescription,
                false, endDate.AddDays(-1), endDate, new List<StepEntity>());
            Assert.Throws<DomainException>(() => tournament.SetStartDate(endDate.AddDays(1)));
        }

        [Test]
        public void set_start_date_equals_to_end_date_should_throw()
        {
            var endDate = DateTime.Now;
            var tournament = TournamentAggregate.Restore(
                _tournamentId, _tournamentName, _tournamentDescription,
                false, endDate.AddDays(-1), endDate, new List<StepEntity>());
            Assert.Throws<DomainException>(() => tournament.SetStartDate(endDate));
        }

        [Test]
        public void set_start_date_should_works()
        {
            var tournament = GetNewTournament();
            var startDate = DateTime.Now;
            tournament.SetStartDate(startDate);
            Assert.AreEqual(startDate, tournament.StartDate);
        }

        [Test]
        public void set_unordered_steps_should_throw()
        {
            var tournament = GetNewTournament();
            var steps = new List<StepEntity>()
            {
                GetStep(0, false),
                GetStep(3, false)
            };
            Assert.Throws<DomainException>(() => tournament.SetSteps(steps));
        }

        [Test]
        public void set_steps_should_works()
        {
            var tournament = GetNewTournament();
            var steps = new List<StepEntity>()
            {
                GetStep(1, false),
                GetStep(2, false)
            };
            tournament.SetSteps(steps);
            for (var i = 0; i < tournament.Steps.Count; i++)
            {
                Assert.AreEqual(steps[i], tournament.Steps[i]);
            }
        }

        [Test]
        public void publish_tournament_without_mandatory_steps_should_throw()
        {
            var steps = new List<StepEntity>() {GetStep(0, true)};
            var tournament = TournamentAggregate.Restore(_tournamentId, _tournamentName, _tournamentDescription, false,
                DateTime.Now, DateTime.Now.AddDays(1), steps);
            Assert.Throws<DomainException>(() => tournament.Publish());
        }

        [Test]
        public void publish_tournament_without_steps_should_throw()
        {
            var tournament = TournamentAggregate.Restore(_tournamentId, _tournamentName, _tournamentDescription, false,
                DateTime.Now, DateTime.Now.AddDays(1), new List<StepEntity>());
            Assert.Throws<DomainException>(() => tournament.Publish());
        }

        [Test]
        public void publish_tournament_without_end_date_should_throw()
        {
            var steps = new List<StepEntity>() {GetStep(0, false)};
            var tournament = TournamentAggregate.Restore(_tournamentId, _tournamentName, _tournamentDescription, false,
                DateTime.Now, null, steps);
            Assert.Throws<DomainException>(() => tournament.Publish());
        }

        [Test]
        public void publish_tournament_should_works()
        {
            var steps = new List<StepEntity> {GetStep(0, false)};
            var startDate = DateTime.Now;
            var tournament = TournamentAggregate.Restore(_tournamentId, _tournamentName, _tournamentDescription, false,
                startDate, startDate.AddDays(1), steps);
            tournament.Publish();
            Assert.AreEqual(true, tournament.IsPublished);
        }

        [Test]
        public void remove_step_on_published_tournament_should_throw()
        {
            var tournament = GetPublishedTournament();
            Assert.Throws<DomainException>(() => tournament.RemoveStep(new StepId(Guid.NewGuid())));
        }

        [Test]
        public void remove_not_found_step_should_throw()
        {
            var tournament = GetNewTournamentWithSteps();
            Assert.Throws<DomainException>(() => tournament.RemoveStep(new StepId(Guid.NewGuid())));
        }

        [Test]
        public void remove_step_should_works()
        {
            var tournament = GetNewTournamentWithSteps();
            var step = tournament.Steps.First();
            tournament.RemoveStep(step.Id);
            CollectionAssert.DoesNotContain(tournament.Steps, step);
        }

        [Test]
        public void add_already_existing_step_should_throw()
        {
            var tournament = GetNewTournamentWithSteps();
            Assert.Throws<DomainException>(() => tournament.AddStep(tournament.Steps.First()));
        }

        [Test]
        public void add_step_on_published_tournament_should_throw()
        {
            var tournament = GetPublishedTournament();
            Assert.Throws<DomainException>(() => tournament.AddStep(GetStep(0, false)));
        }

        [Test]
        public void add_step_should_work()
        {
            var tournament = GetNewTournamentWithSteps();
            var newStep = GetStep(3, false);
            tournament.AddStep(newStep);
            CollectionAssert.Contains(tournament.Steps, newStep);
        }

        [Test]
        public void validate_deletion_should_throw_on_published_and_not_ended_tournament()
        {
            var startDate = DateTime.Now;
            var tournament = GetPublishedTournament(startDate, startDate.AddDays(3));
            Assert.Throws<DomainException>(() =>
                tournament.ValidateDeletion(startDate));
        }

        [Test]
        public void validate_deletion_should_work()
        {
            var startDate = DateTime.Now;
            var tournament = GetPublishedTournament(startDate, startDate.AddDays(1));
            tournament.ValidateDeletion(startDate.AddDays(3));
            Assert.Pass();
        }

        [Test]
        public void update_published_tournament_should_throw()
        {
            var tournament = GetPublishedTournament();
            Assert.Throws<DomainException>(() =>
                tournament.Update("name", "description",true, DateTime.Now, DateTime.Now.AddDays(2)));
        }

        [Test]
        public void update_tournament_should_work()
        {
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(2);
            const string name = "testName";
            const string description = "testDescription";
            const bool isPublished = true;
            var tournament = GetNewTournamentWithSteps();
            tournament.Update(name, description, isPublished, startDate, endDate);
            Assert.AreEqual(startDate, tournament.StartDate);
            Assert.AreEqual(endDate, tournament.EndDate);
            Assert.AreEqual(name, tournament.Name);
            Assert.AreEqual(description, tournament.Description);
            Assert.AreEqual(isPublished, tournament.IsPublished);
        }

        [Test]
        public void update_start_and_end_date_should_throw_with_end_date_not_null_and_start_date_null()
        {
            var tournament = GetNewTournament();
            Assert.Throws<DomainException>(() => tournament.SetStartDateAndEndDate(null, DateTime.Now));
        }

        [Test]
        public void update_start_and_end_date_should_throw_with_end_date_lower_than_start_date()
        {
            var tournament = GetNewTournament();
            Assert.Throws<DomainException>(() =>
                tournament.SetStartDateAndEndDate(DateTime.Now.AddDays(123), DateTime.Now));
        }

        [Test]
        public void update_start_and_end_date_should_work()
        {
            var tournament = GetNewTournament();
            var startDate = DateTime.Now;
            tournament.SetStartDateAndEndDate(startDate, null);
            Assert.Null(tournament.EndDate);
            Assert.AreEqual(startDate, tournament.StartDate);
        }
    }
}