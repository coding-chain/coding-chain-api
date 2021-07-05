using System.Collections.Generic;
using Domain.Contracts;
using Domain.Teams;
using Domain.Users;

namespace Domain.Participations
{
    public class TeamEntity : Entity<TeamId>
    {
        public TeamEntity(TeamId id, IList<UserId> userIds) : base(id)
        {
            UserIds = userIds;
        }

        public IList<UserId> UserIds { get; set; }
    }
}