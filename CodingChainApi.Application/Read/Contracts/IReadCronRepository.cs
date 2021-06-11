using System;
using System.Threading.Tasks;
using Domain.Cron;

namespace Application.Read.Contracts
{
    public interface IReadCronRepository
    {
        public Task<DateTime?> GetLastExecution(string cronName, CronStatus filterStatus);
    }
}