using System;

namespace CodingChainApi.Infrastructure.Models.Cache
{
    public class Function
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string Code { get; set; }
        public int? Order { get; set; }


    }
}