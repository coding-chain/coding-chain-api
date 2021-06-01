﻿using System;
using System.Threading.Tasks;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Services.Cache;
using CodingChainApi.Infrastructure.Settings;
using Domain.Participations;
using Domain.ParticipationStates;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class ParticipationsSessionRepository : IParticipationsSessionsRepository
    {
        private readonly IParticipationRepository _participationRepository;
        private readonly ICache _cache;
        private readonly ICacheSettings _settings;
        public ParticipationsSessionRepository(
            IParticipationRepository participationRepository, ICache cache, ICacheSettings settings)
        {
            _participationRepository = participationRepository;
            _cache = cache;
            _settings = settings;
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

                participationSession = ParticipationSessionAggregate.FromParticipationAggregate(participation);
                _cache.SetCache( participationSession, participationSession.Id,_settings.ParticipationSecondDuration);
            }

            return participationSession;
        }

        public async Task RemoveAsync(ParticipationId id)
        {
            _cache.RemoveCache(id);
            await _participationRepository.RemoveAsync(id);
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