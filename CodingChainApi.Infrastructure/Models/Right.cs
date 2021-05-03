using System;
using System.Collections.Generic;
using Domain.Users;

namespace CodingChainApi.Infrastructure.Models
{
    public class Right
    {
        public Guid Id { get; set; }
        public RightEnum Name { get; set; }
        
        public IList<User> Users { get; set; }
    }
}