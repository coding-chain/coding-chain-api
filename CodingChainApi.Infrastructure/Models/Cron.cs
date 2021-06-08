using System;
using Domain.Cron;

namespace CodingChainApi.Infrastructure.Models
{
    public class Cron
    {
        public Guid Id;
        public string Code;
        public CronStatus Status;

    }
}