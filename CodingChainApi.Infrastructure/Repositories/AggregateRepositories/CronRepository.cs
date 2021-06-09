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
            return ToEntity(await _context.Crons
                .Include(c => c.Status)
                .FirstOrDefaultAsync(cr => cr.Id == id.Value));
        }

        public async Task RemoveAsync(CronId id)
        {
            var cronToBeRemoved = ToModel(await FindByIdAsync(id) ??
                                          throw new InvalidOperationException("cron To be removed not found"));
            _context.Remove(cronToBeRemoved.Result);
            await _context.SaveChangesAsync();
        }

        public Task<CronId> NextIdAsync()
        {
            return new CronId(Guid.NewGuid()).ToTask();
        }

        public async Task<IList<CronAggregate>> GetAllAsync()
        {
            return await _context.Crons
                .Include(cr => cr.Status)
                .Select(cr => ToEntity(cr))
                .ToListAsync();
        }

        public async Task<CronAggregate?> GetCronLastExecution(string cronName, CronStatus filterStatus)
        {
            var statusFilter = _context.CronStatus.FirstOrDefault(cr => cr.Code == filterStatus.Status);
            var cron = await _context.Crons.LastOrDefaultAsync(cr =>
                statusFilter != null && cr.Code == cronName && cr.Status.Id == statusFilter.Id);
            return cron is null ? null : ToEntity(cron);
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
                cron.Status = _context.CronStatus.FirstOrDefault(stat => cronStatus == stat.Code) ??
                              throw new InvalidOperationException();
                //user.Rights = await _context.Rights.Where(r => rightsNames.Contains(r.Name)).ToListAsync();

                return cron;
            }
        }

        private static CronAggregate ToEntity(Cron model)
        {
            return new(
                new CronId(model.Id),
                model.Code,
                model.ExecutedAt,
                new CronStatus(model.Status.Code)
            );
        }
    }
}