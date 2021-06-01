using System.Collections.Generic;
using System.Linq;
using Domain.Participations;
using Domain.Teams;
using Domain.Users;

namespace Domain.ParticipationStates
{
    public class TeamStateEntity : TeamEntity
    {
        public HashSet<ConnectedUserEntity> ConnectedUserEntities { get; }

        public TeamStateEntity(TeamId id, IList<UserId> userIds, IList<ConnectedUserEntity> connectedUserEntities) :
            base(id, userIds)
        {
            ConnectedUserEntities = connectedUserEntities.ToHashSet();
        }

        public ConnectedUserEntity? TeamAdmin => ConnectedUserEntities.FirstOrDefault(u => u.IsAdmin);
    }
}