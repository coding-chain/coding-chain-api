using System;
using System.Collections.Generic;
using Domain.Users;

namespace Application.Read.Users
{
    public record PublicUser(Guid Id, string Username, string Email, IList<Guid> RightIds, IList<Guid> TeamIds);
}