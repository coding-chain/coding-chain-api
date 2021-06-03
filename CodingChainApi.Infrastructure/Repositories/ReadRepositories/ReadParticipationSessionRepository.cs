using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Read.Contracts;
using Application.Read.ParticipationSessions;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Services.Cache;
using Domain.Participations;
using Domain.ParticipationStates;
using Microsoft.AspNetCore.Http;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadParticipationSessionRepository : IReadParticipationSessionRepository
    {
        private readonly ICache _cache;

        public ReadParticipationSessionRepository(ICache cache)
        {
            _cache = cache;
        }

        private static ParticipationSessionNavigation ToParticipationSessionNavigation(
            ParticipationSessionAggregate participation)
            => new ParticipationSessionNavigation(
                participation.Id.Value,
                participation.Team.Id.Value,
                participation.TournamentEntity.Id.Value,
                participation.StepEntity.Id.Value,
                participation.StartDate,
                participation.EndDate,
                participation.CalculatedScore,
                participation.Functions.Select(f => f.Id.Value).ToList(),
                participation.LastError,
                participation.LastOutput,
                participation.ProcessStartTime,
                participation.PassedTestsIds.Select(id => id.Value).ToList()
            );

        public Task<bool> ExistsById(Guid id)
        {
            var participation = _cache.GetCache<ParticipationSessionAggregate>(new ParticipationId(id));
            return (participation is not null).ToTask();
        }

        public async Task<ParticipationSessionNavigation?> GetOneById(Guid id)
        {
            var participation = _cache.GetCache<ParticipationSessionAggregate>(new ParticipationId(id));
            if (participation is null) return null;
            return await ToParticipationSessionNavigation(participation).ToTask();
        }
    }
    
    
}