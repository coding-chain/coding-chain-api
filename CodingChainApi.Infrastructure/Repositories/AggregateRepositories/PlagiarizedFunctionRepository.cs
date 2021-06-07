using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Domain.CodeAnalysis;
using Domain.Participations;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class PlagiarizedFunctionRepository : IPlagiarizedFunctionRepository
    {
        private readonly CodingChainContext _context;

        public PlagiarizedFunctionRepository(CodingChainContext context)
        {
            _context = context;
        }

        public async Task<FunctionId> SetAsync(PlagiarizedFunctionAggregate aggregate)
        {
            var plagiarizedFunction = await ToModel(aggregate);
            _context.PlagiarizedFunctions.Upsert(plagiarizedFunction);
            await _context.SaveChangesAsync();
            return new FunctionId(plagiarizedFunction.Id);
        }

        public async Task<PlagiarizedFunctionAggregate?> FindByIdAsync(FunctionId id)
        {
            return ToEntity(await _context.PlagiarizedFunctions
                .Include(u => u.PlagiarizedFunctions)
                .FirstOrDefaultAsync(u => u.Id == id.Value && u.IsDeleted == false));
        }

        public async Task RemoveAsync(FunctionId id)
        {
            var plagiarizedFunction = await _context.PlagiarizedFunctions.FirstOrDefaultAsync(u => u.Id == id.Value);
            if (plagiarizedFunction is not null)
                plagiarizedFunction.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public Task<FunctionId> NextIdAsync()
        {
            return new FunctionId(Guid.NewGuid()).ToTask();
        }

        public async Task<IList<PlagiarizedFunctionAggregate>> GetAllAsync()
        {
            return await _context.PlagiarizedFunctions
                .Include(u => u.PlagiarizedFunctions)
                .Where(u => !u.IsDeleted)
                .Select(u => ToEntity(u))
                .ToListAsync();
        }

        private async Task<PlagiarizedFunction> FindAsync(Guid id)
        {
            return await _context.PlagiarizedFunctions.FirstOrDefaultAsync(u => u.Id == id);
        }

        private async Task<PlagiarizedFunction> ToModel(PlagiarizedFunctionAggregate aggregate)
        {
            var plagiarizedFunctionIds = aggregate.PlagiarizedFunction.Select(func => func.Id);
            var plagiarizedFunction = await FindAsync(aggregate.Id.Value) ?? new PlagiarizedFunction();
            plagiarizedFunction.Id = aggregate.Id.Value;
            plagiarizedFunction.PlagiarizedFunctions = (IList<PlagiarizedFunctionEntity>) aggregate.PlagiarizedFunction;
            plagiarizedFunction.CheatingFunctionId = aggregate.CheatingFunctionId;
            return plagiarizedFunction;
        }

        private static PlagiarizedFunctionAggregate ToEntity(Models.PlagiarizedFunction model)
        {
            return new(
                new FunctionId(model.Id),
                (List<PlagiarizedFunctionEntity>) model.PlagiarizedFunctions.Select(func =>
                    new PlagiarizedFunctionEntity(func.Id, func.CheatingFunctionId, func.ComparedFunctionId,
                        func.Rate)),
                model.CheatingFunctionId);
        }
    }
}