using System;
using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Models
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public IList<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
    }
}