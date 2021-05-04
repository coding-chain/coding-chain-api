using Domain.Users;

namespace Application.Contracts.IService
{
    public interface ICurrentUserService
    {
        UserId? UserId { get; }
        public UserId ConnectedUserId { get; set; }
    }
}