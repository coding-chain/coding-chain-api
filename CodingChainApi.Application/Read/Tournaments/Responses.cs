using System;
using System.Collections.Generic;

namespace Application.Read.Tournaments
{
    public record TournamentStepNavigation(Guid StepId, Guid TournamentId, bool IsOptional, int Order);

    public record TournamentNavigation(Guid Id, string Name, string Description, bool IsPublished, DateTime? StartDate, DateTime? EndDate, IList<Guid> StepsIds);
}