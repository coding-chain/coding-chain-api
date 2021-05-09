using System;
using System.Collections;
using System.Collections.Generic;

namespace Application.Read.Steps
{

    public record StepNavigation(Guid Id, Guid LanguageId, string Name, string Description, int? MinFunctionsCount,
        int? MaxFunctionsCount, decimal Score, int Difficulty, string HeaderCode,
        IList<Guid> TestIds,
        IList<Guid> TournamentsIds,
        IList<Guid> ParticipationIds);
}