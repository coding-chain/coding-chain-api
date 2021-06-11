using System;
using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Models.Cache
{
    public class Team
    {
        public Guid Id { get; set; }
        public IList<ConnectedUser> ConnectedUsers { get; set; }
        public IList<Guid> UserIds { get; set; }
    }
}