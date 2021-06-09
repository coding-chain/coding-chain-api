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

        public async Task<FunctionId> SetAsync(SuspectFunctionAggregate aggregate)
        {
            var suspectedFunction = await ToModel(aggregate);
            _context.Functions.Upsert(suspectedFunction);
            await _context.SaveChangesAsync();
            return new FunctionId(suspectedFunction.Id);
        }

        public async Task<SuspectFunctionAggregate?> FindByIdAsync(FunctionId id)
        {
            var functionEntity = await GetIncludeQueryable()
                .FirstOrDefaultAsync(f => f.Id == id.Value && f.IsDeleted == false);
            return functionEntity is not null ? ToEntity(functionEntity) : null;
        }

        public async Task RemoveAsync(FunctionId id)
        {
            var function = await FindAsync(id.Value);
            if (function is null) return;
            function.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public Task<FunctionId> NextIdAsync()
        {
            return new FunctionId(Guid.NewGuid()).ToTask();
        }

        public async Task<IList<SuspectFunctionAggregate>> GetAllAsync()
        {
            return await GetIncludeQueryable()
                .Select(f => ToEntity(f))
                .ToListAsync();
        }

        private IQueryable<Function> GetIncludeQueryable()
        {
            return _context.Functions
                .Include(f => f.PlagiarizedFunctions)
                .ThenInclude<Function, PlagiarismFunction, Function>(pF => pF.PlagiarizedFunction);
        }

        private async Task<Function?> FindAsync(Guid id)
        {
            return await GetIncludeQueryable()
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        private async Task<Function> ToModel(SuspectFunctionAggregate aggregate)
        {
            var suspectedFunction = await FindAsync(aggregate.Id.Value) ?? new Function();
            var currentPlagiarizedFunctions = GetCurrentCheatingFunctions(suspectedFunction);
            var removedPlagiarizedFunctions = GetRemovedPlagiarizedFunctions(aggregate, suspectedFunction);
            var newPlagiarizedFunctions = GetNewPlagiarizedFunctions(aggregate, currentPlagiarizedFunctions);
            newPlagiarizedFunctions.ForEach(pF => suspectedFunction.PlagiarizedFunctions.Add(pF));
            removedPlagiarizedFunctions.ForEach(pF => pF.IsDeleted = true);
            return suspectedFunction;
        }

        private static List<PlagiarismFunction> GetNewPlagiarizedFunctions(SuspectFunctionAggregate aggregate,
            IList<PlagiarismFunction> currentPlagiarizedFunctions)
        {
            return aggregate.PlagiarizedFunctions
                .Where(pF => currentPlagiarizedFunctions.All(f =>
                    f.PlagiarizedFunction.Id != pF.Id.Value
                ))
                .Select(pF => new PlagiarismFunction
                {
                    CheatingFunctionId = aggregate.Id.Value, PlagiarizedFunctionId = pF.Id.Value, Rate = pF.Rate,
                    DetectionDate = pF.DetectionDate
                })
                .ToList();
        }

        private static List<PlagiarismFunction> GetRemovedPlagiarizedFunctions(SuspectFunctionAggregate aggregate,
            Function function)
        {
            return function.PlagiarizedFunctions
                .Where(pF => aggregate.PlagiarizedFunctions.All(f =>
                    !pF.IsDeleted
                    && !pF.PlagiarizedFunction.IsDeleted
                    && f.Id.Value != pF.PlagiarizedFunctionId))
                .ToList();
        }


        private IList<PlagiarismFunction> GetCurrentCheatingFunctions(Function function)
        {
            return function.PlagiarizedFunctions
                .Where(pF => !pF.IsDeleted && !pF.PlagiarizedFunction.IsDeleted)
                .ToList();
        }


        private static SuspectFunctionAggregate ToEntity(Function model)
        {
            var plagiarizedFunctions = model.PlagiarizedFunctions
                .Where(func => !func.IsDeleted && !func.PlagiarizedFunction.IsDeleted)
                .Select(func =>
                    new PlagiarizedFunctionEntity(
                        new FunctionId(func.PlagiarizedFunctionId),
                        func.Rate,
                        func.DetectionDate
                    ))
                .ToList();
            return new SuspectFunctionAggregate(new FunctionId(model.Id), plagiarizedFunctions);
        }
    }
}