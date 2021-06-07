using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Read.Contracts;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Services.Cache;
using CodingChainApi.Infrastructure.Settings;
using Domain.Participations;
using Domain.ParticipationStates;
using Domain.StepEditions;
using TestEntity = Domain.Participations.TestEntity;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class ParticipationsSessionRepository : IParticipationsSessionsRepository
    {
        private readonly IParticipationRepository _participationRepository;
        private readonly ICache _cache;
        private readonly ICacheSettings _settings;
        private readonly IReadTestRepository _readTestRepository;

        public ParticipationsSessionRepository(
            IParticipationRepository participationRepository, ICache cache, ICacheSettings settings,
            IReadTestRepository readTestRepository)
        {
            _participationRepository = participationRepository;
            _cache = cache;
            _settings = settings;
            _readTestRepository = readTestRepository;
        }


        public async Task<ParticipationId> SetAsync(ParticipationSessionAggregate aggregate)
        {
            var isCached = _cache.SetCache(aggregate, aggregate.Id, _settings.ParticipationSecondDuration);
            return await aggregate.Id.ToTask();
        }

        public async Task<ParticipationSessionAggregate?> FindByIdAsync(ParticipationId id)
        {
            var participationSession = _cache.GetCache<ParticipationSessionAggregate>(id);
            if (participationSession is null)
            {
                var participation = await _participationRepository.FindByIdAsync(id);
                if (participation is null)
                {
                    return null;
                }

                var tests = await _readTestRepository.GetAllTestNavigationByStepId(participation.StepEntity.Id.Value);
                var testsEntities = tests.Select(t => new TestEntity(new TestId(t.Id), t.Score)).ToList();
                participationSession =
                    ParticipationSessionAggregate.FromParticipationAggregate(participation, testsEntities);
                _cache.SetCache(participationSession, participationSession.Id, _settings.ParticipationSecondDuration);
            }

            return participationSession;
        }

        public async Task RemoveAsync(ParticipationId id)
        {
            await _cache.RemoveCache(id).ToTask();
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