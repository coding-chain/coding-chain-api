using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Cron;

namespace Application.Write.Contracts
{
    public interface ICronRepository: IAggregateRepository<CronId, CronAggregate>
    {
    }
}