using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Application.Contracts.IService;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Domain.CodeAnalysis;
using Domain.Participations;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class SuspectFunctionRepository : ISuspectFunctionRepository
    {
        private readonly CodingChainContext _context;
        private readonly IHashService _hashService;


        public SuspectFunctionRepository(CodingChainContext context, IHashService hashService)
        {
            _context = context;
            _hashService = hashService;
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
                .FirstOrDefaultAsync(f =>
                    f.Id == id.Value && f.IsDeleted == false &&
                    f.PlagiarizedFunctions.Any(pF => !pF.PlagiarizedFunction.IsDeleted));
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
                .ThenInclude(pF => pF.PlagiarizedFunction)
                .Include(f => f.CheatingFunctions)
                .ThenInclude(pF => pF.CheatingFunction);
        }

        private async Task<Function?> FindAsync(Guid id)
        {
            return await GetIncludeQueryable()
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        private async Task<Function> ToModel(SuspectFunctionAggregate aggregate)
        {
            var suspectedFunction = await FindAsync(aggregate.Id.Value) ?? new Function();
            var currentPlagiarizedFunctions = GetCurrentPlagiarizedFunctions(suspectedFunction);
            var newPlagiarizedFunctions = GetNewPlagiarizedFunctions(aggregate, currentPlagiarizedFunctions);
            newPlagiarizedFunctions.ForEach(pF => suspectedFunction.PlagiarizedFunctions.Add(pF));
            currentPlagiarizedFunctions.ForEach(pF =>
            {
                var func = aggregate.PlagiarizedFunctions.First(f => f.Id.Value == pF.PlagiarizedFunctionId);
                pF.IsValid = func.IsValid;
                pF.Rate = func.Rate;
                pF.DetectionDate = func.DetectionDate;
                pF.CheatingFunctionHash = func.SuspectHash;
                pF.PlagiarizedFunctionHash = func.Hash;
                pF.PlagiarizedFunctionHash = func.SuspectHash;
            });
            return suspectedFunction;
        }

        private static List<PlagiarismFunction> GetNewPlagiarizedFunctions(SuspectFunctionAggregate aggregate,
            IList<PlagiarismFunction> currentPlagiarizedFunctions)
        {
            return aggregate.PlagiarizedFunctions
                .Where(pF =>
                {
                    var currentFunc = currentPlagiarizedFunctions.FirstOrDefault(f =>
                        f.PlagiarizedFunction.Id == pF.Id.Value
                    );
                    return currentFunc?.CheatingFunctionHash != pF.SuspectHash &&
                           currentFunc?.PlagiarizedFunctionHash != pF.Hash;
                })
                .Select(pF => new PlagiarismFunction
                {
                    CheatingFunctionId = aggregate.Id.Value,
                    PlagiarizedFunctionId = pF.Id.Value,
                    Rate = pF.Rate,
                    CheatingFunctionHash = pF.SuspectHash,
                    PlagiarizedFunctionHash = pF.Hash,
                    DetectionDate = pF.DetectionDate,
                    IsValid = pF.IsValid
                })
                .ToList();
        }

        private List<PlagiarismFunction> GetCurrentPlagiarizedFunctions(Function function)
        {
            return GetFirstPlagiarizedFunctions(function);
        }

        private List<PlagiarismFunction> GetFirstPlagiarizedFunctions(Function suspectFunction)
        {
            return suspectFunction.PlagiarizedFunctions
                .Where(func => !func.PlagiarizedFunction.IsDeleted)
                .GroupBy(func => func.PlagiarizedFunctionId)
                .Select(func => func.OrderByDescending(f => f.DetectionDate).First())
                .ToList();
        }

        private SuspectFunctionAggregate ToEntity(Function model)
        {
            var plagiarizedFunctions = GetFirstPlagiarizedFunctions(model)
                .Select(func =>
                {
                    return new PlagiarizedFunctionEntity(
                        new FunctionId(func.PlagiarizedFunctionId),
                        func.Rate,
                        func.PlagiarizedFunctionHash,
                        func.CheatingFunctionHash,
                        func.DetectionDate
                    );
                })
                .ToList();
            return SuspectFunctionAggregate.Restore(new FunctionId(model.Id), plagiarizedFunctions);
        }
    }
}