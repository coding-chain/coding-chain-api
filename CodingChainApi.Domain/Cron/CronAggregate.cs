using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Contracts;
using Domain.Exceptions;

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
        public string Code { get; private set; }
        public DateTime ExecutedAt { get; private set; }
        public CronStatus Status { get; private set; }
        public DateTime? FinishedAt { get; private set; }

        private CronAggregate(CronId id, string code, DateTime executedAt, CronStatus status, DateTime? finishedAt) :
            base(id)
        {
            Id = id;
            Code = code;
            ExecutedAt = executedAt;
            Status = status;
            FinishedAt = finishedAt;
        }

        public static CronAggregate CreateNew(CronId id, string code, DateTime executedAt, CronStatus status,
            DateTime? finishedAt)
        {
            var errors = new List<string>();
            if (!status.Status.Equals(CronStatusEnum.Executing))
            {
                errors.Add($"Cron cannot be created with success or error status");
            }

            if (errors.Any())
            {
                throw new DomainException(errors);
            }

            return new CronAggregate(id, code, executedAt, status, finishedAt);
        }

        public static CronAggregate Restore(CronId id, string code, DateTime executedAt, CronStatus status,
            DateTime? finishedAt)
        {
            return new(id, code, executedAt, status, finishedAt);
        }

        public void FinishCron(CronStatus newStatus, DateTime finishedAt)
        {
            Status = newStatus;
            FinishedAt = finishedAt;
        }
    }
}