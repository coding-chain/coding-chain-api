using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Contracts.Dtos;
using Application.Read.Contracts;
using Application.Read.Functions;
using Application.Read.Plagiarism;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
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

        public async Task<IList<Function>> GetAllFunctionsNotPaginated()
        {
            return await _context.Functions
                .Where(func => !func.IsDeleted)
                .Select(func => new Function(func.Id, func.Code))
                .ToListAsync();
        }

        public async Task<IList<Function>> GetAllFunctionFilterOnModifiedDate(DateTime? dateFilter)
        {
            return await _context.UserFunctions.Where(userFunc => userFunc.LastModificationDate > dateFilter)
                .Select(userFunc =>
                    new Function(userFunc.FunctionId, userFunc.Function.Code))
                .ToListAsync();
        }
    }
}