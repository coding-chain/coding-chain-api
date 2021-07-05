using System;

namespace CodingChainApi.Infrastructure.Models.Cache
{
    public class ConnectedUser
    {
        public Guid Id { get; set; }
        public bool IsAdmin { get; set; } = false;
        public int ConnectionCount { get; set; } = 0;
    }
}