using System;
using System.Collections.Generic;
using Castle.DynamicProxy.Contributors;

namespace Application.Read.Teams
{
    public record MemberNavigation(Guid UserId, Guid TeamId, bool IsAdmin, DateTime JoinDate, DateTime? LeaveDate); 
    public record TeamNavigation(Guid Id, string Name, IList<Guid> MembersIds);
}