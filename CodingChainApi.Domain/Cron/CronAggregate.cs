using System;
using Domain.Contracts;

namespace Domain.Cron
{
    public record CronId(Guid Value) : IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    public class CronAggregate
    {
        
    }
}