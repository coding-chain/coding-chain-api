using System;
using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool IsDeleted { get; set; }

        public IList<Right> Rights { get; set; } = new List<Right>();

        public IList<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
        public IList<UserFunction> UserFunctions { get; set; } = new List<UserFunction>();
    }
}