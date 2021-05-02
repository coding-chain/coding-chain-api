using System;

namespace Application.Read.Users
{
    public record ConnectedUser(Guid Id, string Username, string Email);
}