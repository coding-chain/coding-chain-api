using System;
using System.Collections.Generic;
using Castle.DynamicProxy.Contributors;

namespace Application.Read.Teams
{
    public record TeamNavigation(Guid Id, string Name, IList<Guid> MemberIds);
}