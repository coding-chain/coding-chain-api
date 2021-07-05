using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Read.Contracts;
using Application.Read.ParticipationSessions;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Models.Cache;
using CodingChainApi.Infrastructure.Services.Cache;
using Domain.Participations;
using Domain.ParticipationSessions;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadParticipationSessionRepository : IReadParticipationSessionRepository
    {
        private readonly ICache _cache;

        public ReadParticipationSessionRepository(ICache cache)
        {
            _cache = cache;
        }

        public async Task<bool> ExistsById(Guid id)
        {
            var participation = await _cache.GetCache<ParticipationSession>(id);
            return (participation is not null);
        }

        public async Task<ParticipationSessionNavigation?> GetOneById(Guid id)
        {
            var participation = await _cache.GetCache<ParticipationSession>(id);
            if (participation is null) return null;
            return  ToParticipationSessionNavigation(participation);
        }

        private static ParticipationSessionNavigation ToParticipationSessionNavigation(
            ParticipationSession participation)
        {
            return new(
                participation.Id,
                participation.Team.Id,
                participation.Tournament.Id,
                participation.Step.Id,
                participation.StartDate,
                participation.EndDate,
                participation.CalculatedScore,
                participation.Functions.Select(f => f.Id).ToList(),
                participation.LastError,
                participation.LastOutput,
                participation.ProcessStartTime,
                participation.PassedTestsIds.Select(id => id).ToList(),
                participation.IsReady
            );
        }
    }
}