using Domain.Contracts;
using Domain.Users;

namespace Domain.ParticipationStates
{
    public class ConnectedUserEntity : Entity<UserId>
    {
        public bool IsAdmin { get; set; }
        public int ConnectionCount { get; set; } = 0;

        public ConnectedUserEntity(UserId id, bool isAdmin) : base(id)
        {
            IsAdmin = isAdmin;
        }
    }
}