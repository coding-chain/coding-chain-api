using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Read.Contracts;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using CronStatus = Domain.Cron.CronStatus;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadCronRepository: IReadCronRepository
    {
        private readonly CodingChainContext _context;

        public ReadCronRepository(CodingChainContext context)
        {
            _context = context;
        }

        private static IQueryable<Cron> GetInclude(IQueryable<Cron> cronQuery) => cronQuery.Include(c => c.Status);
        public async Task<DateTime?> GetLastExecution(string cronName, CronStatus filterStatus)
        {
            var cron = await GetInclude(_context.Crons)
                .Where(c => c.Status.Code == filterStatus.Status && c.Code == cronName)
                .OrderByDescending(c => c.FinishedAt)
                .FirstOrDefaultAsync();
            return cron?.FinishedAt;

        }
    }
}