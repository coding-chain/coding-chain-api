using System;

namespace CodingChainApi.Infrastructure.Models
{
    public class Test
    {
        public Guid Id { get; set; }
        public string OutputValidator { get; set; }
        public string InputGenerator { get; set; }
        public decimal Score { get; set; }
        public bool IsDeleted { get; set; }

        public Step Step { get; set; }
    }
}