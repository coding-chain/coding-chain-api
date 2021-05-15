using System;

namespace CodingChainApi.Infrastructure.Services.Processes
{
    public record ProcessEndResult(Guid ParticipationId, string? Errors, string? Output);
}