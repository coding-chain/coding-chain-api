using System;
using System.Collections.Generic;

namespace Application.Read.ParticipationSessions
{
    public record ParticipationSessionNavigation(
        Guid Id,
        Guid TeamId,
        Guid TournamentId,
        Guid StepId,
        DateTime StartDate,
        DateTime? EndDate,
        decimal CalculatedScore,
        IList<Guid> FunctionsIds,
        string? LastError,
        string? LastOutput,
        DateTime? ProcessStartTime
        );
}