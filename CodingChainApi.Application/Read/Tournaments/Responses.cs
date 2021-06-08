using System;
using System.Collections.Generic;

namespace Application.Read.Tournaments
{
    public record TournamentStepNavigation(Guid StepId, Guid TournamentId, bool IsOptional, int Order,
        Guid LanguageId, string Name, string Description, int? MinFunctionsCount,
        int? MaxFunctionsCount, decimal Score, int Difficulty, string? HeaderCode, bool IsPublished,
        IList<Guid> TestIds,
        IList<Guid> ParticipationIds, IList<Guid> TournamentIds);

    public record TournamentNavigation(Guid Id, string Name, string Description, bool IsPublished, DateTime? StartDate,
        DateTime? EndDate, IList<Guid> StepsIds, IList<Guid> ParticipationsIds);
}