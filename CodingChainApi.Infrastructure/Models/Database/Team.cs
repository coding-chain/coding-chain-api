using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingChainApi.Infrastructure.Models
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public IList<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
        public IList<Participation> Participations { get; set; } = new List<Participation>();

        public IList<Guid> ActiveParticipationsIds => ActiveParticipations
            .Select(p => p.Id)
            .ToList();

        public IList<Participation> ActiveParticipations => Participations
            .Where(p => !p.Deactivated && !p.Tournament.IsDeleted && p.Tournament.IsPublished && !p.Step.IsDeleted)
            .ToList();

        public IList<Guid> ActiveMembersIds => ActiveMembers
            .Select(uT => uT.Id).ToList();

        public IList<UserTeam> ActiveMembers => UserTeams
            .Where(uT => uT.LeaveDate == null && !uT.User.IsDeleted)
            .ToList();
    }
}