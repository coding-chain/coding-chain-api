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

    public class CronAggregate : Aggregate<CronId>
    {
        public string Code;
        public DateTime ExecutedAt;
        public CronStatus Status;

        public CronAggregate(CronId id, string code, DateTime executedAt, CronStatus status) : base(id)
        {
            Id = id;
            Code = code;
            ExecutedAt = executedAt;
            Status = status;
        }
    }
}