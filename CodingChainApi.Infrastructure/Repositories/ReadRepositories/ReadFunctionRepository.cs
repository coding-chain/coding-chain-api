using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Read.Contracts;
using Application.Read.Functions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadFunctionRepository : IReadFunctionRepository
    {
        private readonly CodingChainContext _context;

        public ReadFunctionRepository(CodingChainContext context)
        {
            _context = context;
        }

        private IQueryable<Function> Include() => _context.Functions
            .Include(f => f.Participation)
            .Include(f => f.UserFunctions);

        private static FunctionNavigation ToFunctionNavigation(Function func) => new FunctionNavigation(
            func.Id,
            func.Participation.Id,
            func.Code,
            func.Order,
            func.UserFunctions.Select(uf => uf.UserId).ToList()
        );

        public async Task<FunctionNavigation?> GetById(Guid id, bool includeDeleted)
        {
            var function = await GetFunctionById(id, includeDeleted);
            return function is null ? null : ToFunctionNavigation(function);
        }

        private async Task<Function?> GetFunctionById(Guid id, bool includeDeleted)
        {
            var function = await Include()
                .FirstOrDefaultAsync(f => (includeDeleted == true || !f.IsDeleted) && f.Id == id);
            return function;
        }

        public async Task<Guid?> GetLastEditorIdById(Guid id, bool includeDeleted)
        {
            var function = await GetFunctionById(id, includeDeleted);
            var userFunction = function?.UserFunctions.OrderByDescending(f => f.LastModificationDate).FirstOrDefault();
            return userFunction?.UserId;
        }
    }
}