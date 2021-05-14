using System;
using Domain.Users;

namespace Application.Read.Rights
{
    public record RightNavigation(Guid Id, RightEnum Name);
}