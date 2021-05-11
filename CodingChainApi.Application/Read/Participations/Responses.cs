using System;
using System.Collections.Generic;

namespace Application.Read.Participations
{
    public record ParticipationNavigation(Guid Id, Guid TeamId, Guid TournamentId, Guid StepId, DateTime StartDate,
        DateTime? EndDate, decimal CalculatedScore, IList<Guid> FunctionsIds);
    
}