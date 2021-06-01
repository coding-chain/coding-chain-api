using System;

namespace Application.Read.FunctionSessions
{
    public record FunctionSessionNavigation(Guid Id, Guid UserId, string Code, DateTime lastModificationDate, int? Order );
}