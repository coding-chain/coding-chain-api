using System;
using System.Collections.Generic;
using Domain.Cron;

namespace CodingChainApi.Infrastructure.Models
{
    public class CronStatus
    {
        public Guid Id{ get; set; }
        public CronStatusEnum Code { get; set; }
        public IList<Cron> Crons { get; set; }
    }
}