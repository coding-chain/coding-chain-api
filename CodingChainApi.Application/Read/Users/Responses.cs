using System;

namespace Application.Read.Users
{
    public record PublicUser(Guid Id, string Username, string Email);
}