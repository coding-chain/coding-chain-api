using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;

namespace Domain.Users
{
    public record UserId(Guid Value) : IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class UserAggregate : Aggregate<UserId>
    {
        public string Email { get; private set; }
        public string Username { get; private set; }
        private List<Right> _rights;

        public UserAggregate(UserId id, string password, IList<Right> rights, string email, string username) : base(id)
        {
            Password = password;
            _rights = rights.ToList();
            Email = email;
            Username = username;
        }

        public string Password { get; private set; }

        public IReadOnlyList<Right> Rights => _rights.AsReadOnly();


        public void SetMandatoryRoles(IEnumerable<UserAggregate> existingUsers)
        {
            var roles = new List<Right>
            {
                new(RightEnum.User)
            };
            if (!existingUsers.Any(u => u._rights.Any(r => r.Name == RightEnum.Admin)))
                roles.Add(new Right(RightEnum.Admin));

            _rights = roles;
        }

        public IList<Right> GetNotMatchingRoles(IList<Right> roles)
        {
            return roles.Where(r => !_rights.Contains(r)).ToList();
        }

        public void UpdateUser(string? email, string? username, string? password)
        {
            if (email is not null)
                Email = email;

            if (username is not null)
                Username = username;

            if (password is not null)
                Password = password;
        }
    }
}