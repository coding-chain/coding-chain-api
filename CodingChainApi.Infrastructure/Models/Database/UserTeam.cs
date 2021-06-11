using System;

namespace CodingChainApi.Infrastructure.Models
{
    public class UserTeam
    {
        public Guid Id { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid TeamId { get; set; }
        public Team Team { get; set; }
    }
}