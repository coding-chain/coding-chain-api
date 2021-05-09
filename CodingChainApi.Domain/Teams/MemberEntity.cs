using Domain.Contracts;
using Domain.Users;

namespace Domain.Teams
{
    public class MemberEntity : Entity<UserId>
    {
        public bool IsAdmin { get; set; }

        public MemberEntity(UserId id, bool isAdmin) : base(id)
        {
            IsAdmin = isAdmin;
        }
    }
}