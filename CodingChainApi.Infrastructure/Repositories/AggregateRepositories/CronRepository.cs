using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Domain.Cron;
using Microsoft.EntityFrameworkCore;
using CronStatus = Domain.Cron.CronStatus;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class CronRepository : ICronRepository
    {
        private readonly CodingChainContext _context;

        public CronRepository(CodingChainContext context)
        {
            _context = context;
        }

        public async Task<CronId> SetAsync(CronAggregate aggregate)
        {
            var cron = await ToModel(aggregate);
            _context.Crons.Upsert(cron);
            await _context.SaveChangesAsync();
            return new CronId(cron.Id);
        }

        public async Task<CronAggregate?> FindByIdAsync(CronId id)
        {
            var cron = await _context.Crons
                .Include(c => c.Status)
                .FirstOrDefaultAsync(cr => cr.Id == id.Value);
            return cron is null ? null : ToAggregate(cron);
        }

        public async Task RemoveAsync(CronId id)
        {
            var cron = await _context.Crons
                .FirstOrDefaultAsync(c => c.Id == id.Value);
            if (cron is not null)
                _context.Remove(cron);
            await _context.SaveChangesAsync();
        }

        public Task<CronId> NextIdAsync()
        {
            return new CronId(Guid.NewGuid()).ToTask();
        }

        private async Task<Cron?> FindAsync(Guid id)
        {
            return await _context.Crons.FirstOrDefaultAsync(u => u.Id == id);
        }

        private async Task<Cron> ToModel(CronAggregate aggregate)
        {
            {
                var cronStatus = aggregate.Status.Status;
                var cron = await FindAsync(aggregate.Id.Value) ?? new Cron();
                cron.Id = aggregate.Id.Value;
                cron.Code = aggregate.Code;
                cron.ExecutedAt = aggregate.ExecutedAt;
                cron.Status = _context.CronStatus.FirstOrDefault(stat => cronStatus == stat.Code) ??
                              throw new InvalidOperationException();
                cron.FinishedAt = aggregate.FinishedAt;

                return cron;
            }
        }

        private static CronAggregate ToAggregate(Cron model)
        {
            return CronAggregate.Restore(new CronId(model.Id),
                model.Code,
                model.ExecutedAt,
                new CronStatus(model.Status.Code),
                model.FinishedAt);
        }
    }
}