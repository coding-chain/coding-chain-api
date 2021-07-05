using System;
using System.Collections.Generic;

namespace Application.Read.Users
{
    public record PublicUser(Guid Id, string Username, string Email, IList<Guid> RightIds, IList<Guid> TeamIds);
}