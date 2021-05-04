using System;

namespace CodingChainApi.Infrastructure.Models
{
    public class UserFunction
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Function Function { get; set; }
        public Guid FunctionId { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}