using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Read.Contracts;
using Application.Read.Tests;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Models.Cache;
using CodingChainApi.Infrastructure.Services.Cache;
using CodingChainApi.Infrastructure.Settings;
using Domain.Participations;
using Domain.ParticipationSessions;
using Domain.StepEditions;
using Domain.Teams;
using Domain.Tournaments;
using Domain.Users;
using StepEntity = Domain.Participations.StepEntity;
using TestEntity = Domain.ParticipationSessions.TestEntity;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class ParticipationsSessionRepository : IParticipationsSessionsRepository
    {
        private readonly ICache _cache;
        private readonly IParticipationRepository _participationRepository;
        private readonly IReadTestRepository _readTestRepository;
        private readonly ICacheSettings _settings;

        public ParticipationsSessionRepository(
            IParticipationRepository participationRepository, ICache cache, ICacheSettings settings,
            IReadTestRepository readTestRepository)
        {
            _participationRepository = participationRepository;
            _cache = cache;
            _settings = settings;
            _readTestRepository = readTestRepository;
        }

        private ParticipationSession ToModel(ParticipationSessionAggregate aggregate) => new()
        {
            Id = aggregate.Id.Value,
            Functions = aggregate.Functions.Select(ToFunctionModel).ToList(),
            Step = ToStepModel(aggregate.StepSessionEntity),
            Tournament = ToTournamentModel(aggregate.TournamentEntity),
            Team = ToTeamModel(aggregate.ConnectedTeam),
            CalculatedScore = aggregate.CalculatedScore,
            EndDate = aggregate.EndDate,
            IsReady = aggregate.IsReady,
            LastError = aggregate.LastError,
            LastOutput = aggregate.LastOutput,
            StartDate = aggregate.StartDate,
            PassedTestsIds = aggregate.PassedTestsIds.Select(id => id.Value).ToList(),
            ProcessStartTime = aggregate.ProcessStartTime
        };


        private static Team ToTeamModel(TeamSessionEntity team) => new()
        {
            Id = team.Id.Value,
            ConnectedUsers = team.ConnectedUserEntities.Select(ToConnectedUser).ToList(),
            UserIds = team.UserIds.Select(id => id.Value).ToList()
        };

        private static ConnectedUser ToConnectedUser(ConnectedUserEntity user) => new()
        {
            Id = user.Id.Value,
            ConnectionCount = user.ConnectionCount,
            IsAdmin = user.IsAdmin
        };

        private static Tournament ToTournamentModel(TournamentEntity tournament) => new()
        {
            Id = tournament.Id.Value,
            IsPublished = tournament.IsPublished
        };

        private static Step ToStepModel(StepSessionEntity step) => new()
        {
            Id = step.Id.Value,
            TournamentIds = step.TournamentIds.Select(s => s.Value).ToList(),
            Tests = step.Tests.Select(ToTestModel).ToList()
        };

        private static Test ToTestModel(TestEntity test) => new()
        {
            Id = test.Id.Value,
            Score = test.Score
        };

        private static Function ToFunctionModel(FunctionEntity function) => new()
        {
            Code = function.Code,
            Id = function.Id.Value,
            Order = function.Order,
            UserId = function.UserId.Value,
            LastModificationDate = function.LastModificationDate
        };


        public async Task<ParticipationId> SetAsync(ParticipationSessionAggregate aggregate)
        {
            var isCached =
                await _cache.SetCache(ToModel(aggregate), aggregate.Id.Value,
                    _settings.ParticipationSecondDuration);
            return aggregate.Id;
        }

        public async Task<ParticipationSessionAggregate?> FindByIdAsync(ParticipationId id)
        {
            var cacheParticipation = await _cache.GetCache<ParticipationSession>(id);
            if (cacheParticipation is not null) return ToAggregate(cacheParticipation);

            var participation = await _participationRepository.FindByIdAsync(id);
            if (participation is null) return null;

            var tests = await _readTestRepository.GetAllTestNavigationByStepId(participation.StepEntity.Id.Value);
            var participationSession = ToAggregate(participation, tests);
            await _cache.SetCache(ToModel(participationSession), participationSession.Id.Value,
                _settings.ParticipationSecondDuration);
            return participationSession;
        }

        private static ParticipationSessionAggregate ToAggregate(ParticipationAggregate participation,
            IList<TestNavigation> tests) =>
            ParticipationSessionAggregate.Restore(
                participation.Id,
                ToTeamSessionEntity(participation.Team),
                participation.TournamentEntity,
                ToStepSessionEntity(participation.StepEntity, tests),
                participation.StartDate,
                participation.EndDate,
                participation.CalculatedScore,
                false,
                participation.Functions.ToList()
            );

        private static StepSessionEntity ToStepSessionEntity(StepEntity step, IList<TestNavigation> tests) =>
            new(
                step.Id,
                step.TournamentIds,
                tests.Select(ToTestEntity).ToList()
            );

        private static TestEntity ToTestEntity(TestNavigation test) => new(
            new TestId(test.Id), test.Score);

        private static TeamSessionEntity ToTeamSessionEntity(TeamEntity team) => new(
            team.Id,
            team.UserIds,
            new List<ConnectedUserEntity>()
        );

        private static ParticipationSessionAggregate ToAggregate(ParticipationSession participation) =>
            ParticipationSessionAggregate.Restore(
                new ParticipationId(participation.Id),
                ToTeamEntity(participation.Team),
                ToTournamentEntity(participation.Tournament),
                ToStepSessionEntity(participation.Step),
                participation.StartDate,
                participation.EndDate,
                participation.CalculatedScore,
                participation.IsReady,
                participation.Functions.Select(ToFunctionEntity).ToList()
            );

        private static FunctionEntity ToFunctionEntity(Function function) => new FunctionEntity(
            new FunctionId(function.Id),
            new UserId(function.UserId),
            function.Code,
            function.LastModificationDate,
            function.Order
        );

        private static StepSessionEntity ToStepSessionEntity(Step step) => new StepSessionEntity(
            new StepId(step.Id),
            step.TournamentIds.Select(id => new TournamentId(id)).ToList(),
            step.Tests.Select(ToTestEntity).ToList()
        );

        private static TestEntity ToTestEntity(Test test) => new TestEntity(
            new TestId(test.Id),
            test.Score
        );

        private static TeamSessionEntity ToTeamEntity(Team team) => new TeamSessionEntity(
            new TeamId(team.Id),
            team.UserIds.Select(id => new UserId(id)).ToList(),
            team.ConnectedUsers.Select(ToConnectedUserEntity).ToList()
        );

        private static ConnectedUserEntity ToConnectedUserEntity(ConnectedUser user) => new ConnectedUserEntity(
            new UserId(user.Id),
            user.IsAdmin
        );

        private static TournamentEntity ToTournamentEntity(Tournament tournament) => new TournamentEntity(
            new TournamentId(tournament.Id),
            tournament.IsPublished
        );

        public async Task RemoveAsync(ParticipationId id)
        {
            await _cache.RemoveCache(id);
        }

        public Task<ParticipationId> NextIdAsync()
        {
            return _participationRepository.NextIdAsync();
        }

        public Task<FunctionId> GetNextFunctionId()
        {
            return new FunctionId(Guid.NewGuid()).ToTask();
        }
    }
}