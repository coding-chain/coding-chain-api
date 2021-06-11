using System.Collections.Generic;
using System.Linq;
using Domain.Participations;
using Domain.Teams;
using Domain.Users;

namespace Domain.ParticipationSessions
{
    public class TeamSessionEntity : TeamEntity
    {
        public TeamSessionEntity(TeamId id, IList<UserId> userIds, IList<ConnectedUserEntity> connectedUserEntities) :
            base(id, userIds)
        {
            ConnectedUserEntities = connectedUserEntities.ToHashSet();
        }

        public HashSet<ConnectedUserEntity> ConnectedUserEntities { get; }

        public ConnectedUserEntity? TeamAdmin => ConnectedUserEntities.FirstOrDefault(u => u.IsAdmin);
    }
}