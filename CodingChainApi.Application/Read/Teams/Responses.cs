using System;
using System.Collections.Generic;

namespace Application.Read.Teams
{
    public record MemberNavigation(Guid UserId, Guid TeamId, bool IsAdmin, DateTime JoinDate, DateTime? LeaveDate);

    public record TeamNavigation(Guid Id, string Name, IList<Guid> MembersIds, IList<Guid> ParticipationsIds);
}