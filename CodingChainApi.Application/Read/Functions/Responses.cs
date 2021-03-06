using System;
using System.Collections.Generic;

namespace Application.Read.Functions
{
    public record FunctionNavigation(Guid Id, Guid ParticipationId, string Code, int? Order, IList<Guid> UserIds);
}