using Domain.Contracts;
using Domain.Users;

namespace Domain.ParticipationSessions
{
    public class ConnectedUserEntity : Entity<UserId>
    {
        public ConnectedUserEntity(UserId id, bool isAdmin) : base(id)
        {
            IsAdmin = isAdmin;
        }

        public bool IsAdmin { get; set; }
        public int ConnectionCount { get; set; } = 0;
    }
}