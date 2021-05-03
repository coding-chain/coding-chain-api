using System;
using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Models
{
    public class Function
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
        public IList<UserFunction> UserFunctions { get; set; } = new List<UserFunction>();

    }
}