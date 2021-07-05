using System;
using Domain.Cron;

namespace CodingChainApi.Infrastructure.Models
{
    public class Cron
    {
        public Guid Id{ get; set; }
        public string Code { get; set; }
        public CronStatus Status{ get; set; }
        public DateTime ExecutedAt{ get; set; }
        public DateTime? FinishedAt{ get; set; }
    }
}