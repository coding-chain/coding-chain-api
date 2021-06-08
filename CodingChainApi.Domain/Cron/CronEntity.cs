using System;
using Domain.Contracts;

namespace Domain.Cron
{
    public class CronEntity : Entity<CronId>
    {
        public string Name;

        public CronEntity(CronId id, string name) : base(id)
        {
            Name = name;
        }
    }
}