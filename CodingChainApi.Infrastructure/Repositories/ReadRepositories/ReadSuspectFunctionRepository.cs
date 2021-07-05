using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Contracts.Dtos;
using Application.Read.Contracts;
using Application.Read.Functions;
using Application.Read.Plagiarism;
using Application.Read.Plagiarism.Handlers;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Function = CodingChainApi.Infrastructure.Models.Function;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadSuspectFunctionRepository : IReadSuspectFunctionRepository
    {
        private readonly CodingChainContext _context;

        public ReadSuspectFunctionRepository(CodingChainContext context)
        {
            _context = context;
        }

        private static Expression<Func<UserFunction, bool>> ToPredicate(GetFunctionsQuery query,
            bool searchSuspect = false) => userFunction
            => !userFunction.Function.IsDeleted && !userFunction.User.IsDeleted
                                                && (query.LanguageIdFilter == null ||
                                                    userFunction.Function.Participation.Step.ProgrammingLanguage.Id ==
                                                    query.LanguageIdFilter)
                                                && (query.LowerThanDateFilter == null ||
                                                    query.LowerThanDateFilter > userFunction.LastModificationDate)
                                                && (query.GreaterThanDateFilter == null ||
                                                    query.GreaterThanDateFilter < userFunction.LastModificationDate)
                                                && (query.ExcludedUserIdFilter == null ||
                                                    query.ExcludedUserIdFilter != userFunction.UserId)
                                                && (query.FunctionsIdsFilter == null
                                                    || query.FunctionsIdsFilter.Any(id =>
                                                        id == userFunction.FunctionId));

        private IQueryable<UserFunction> GetIncludeUserFunction() => _context.UserFunctions
            .Include(uF => uF.User)
            .Include(uF => uF.Function)
            .ThenInclude(f => f.PlagiarizedFunctions)
            .Include(uF => uF.Function)
            .ThenInclude(f => f.CheatingFunctions)
            .Include(uF => uF.Function)
            .ThenInclude(f => f.Participation)
            .ThenInclude(p => p.Step)
            .ThenInclude(s => s.ProgrammingLanguage);

        public async Task<IList<SuspectFunctionNavigation>> GetAllLastFunctionFiltered(GetFunctionsQuery query)
        {
            var functions = await GetUsersFunctionsFiltered(query);
            return functions.Select(ToSuspectFunctionNavigation).ToList();
        }

        public async Task<IPagedList<PlagiarizedFunctionNavigation>> GetPlagiarizedFunctions(
            GetLastPlagiarizedFunctionsByFunctionQuery query)
        {
            var functions = await GetIncludeUserFunction()
                .Where(uF => !uF.User.IsDeleted && !uF.Function.IsDeleted && uF.Function.CheatingFunctions
                    .Any(cF => !cF.CheatingFunction.IsDeleted &&  cF.IsValid == null && cF.CheatingFunctionId == query.FunctionId))
                .ToListAsync();
            functions = functions.GroupBy(uF => uF.FunctionId)
                .Select(userFuncGrp =>
                    userFuncGrp.OrderByDescending(function => function.LastModificationDate).First()
                )
                .ToList();
            return functions.Select(uF => ToPlagiarizedFunctionNavigation(uF, query.FunctionId)).FromEnumerable(query);
        }
        private async Task<List<UserFunction>> GetUsersFunctionsFiltered(GetFunctionsQuery query)
        {
            var functions = await GetIncludeUserFunction()
                .Where(ToPredicate(query))
                .ToListAsync();
            return OrderUsersFunctionsByLastModificationDate(functions);
        }

        private async Task<List<UserFunction>> GetSuspectUsersFunctionsFiltered(GetFunctionsQuery query)
        {
            var functions = await GetIncludeUserFunction()
                .Where(ToPredicate(query))
                .Where(uF => uF.Function.PlagiarizedFunctions
                    .Any(pF => pF.IsValid == null && !pF.PlagiarizedFunction.IsDeleted))
                .ToListAsync();
            return OrderUsersFunctionsByLastModificationDate(functions);
        }

        private static List<UserFunction> OrderUsersFunctionsByLastModificationDate(IEnumerable<UserFunction> functions)
        {
            return functions.GroupBy(uF => uF.FunctionId)
                .Select(userFuncGrp =>
                    userFuncGrp.OrderByDescending(function => function.LastModificationDate).First()
                )
                .ToList();
        }

        public async Task<IPagedList<SuspectFunctionNavigation>> GetPaginatedLastSuspectFunctionsFiltered(
            GetFunctionsQuery query)
        {
            var functions = await GetSuspectUsersFunctionsFiltered(query);
            return functions.Select(ToSuspectFunctionNavigation)
                .FromEnumerable(query);
        }

        private static FunctionCodeNavigation ToFunctionCodeNavigation(UserFunction userFunction) =>
            new(
                userFunction.FunctionId,
                userFunction.UserId,
                userFunction.Function.Participation.Step.ProgrammingLanguage.Id,
                userFunction.LastModificationDate,
                userFunction.Function.Code
            );

        private static PlagiarizedFunctionNavigation ToPlagiarizedFunctionNavigation(UserFunction userFunction,
            Guid cheatingFunctionId)
        {
            var cheatingFunction = userFunction.Function.CheatingFunctions
                .Where(pF => pF.CheatingFunctionId == cheatingFunctionId)
                .OrderByDescending(pF => pF.DetectionDate)
                .First();
            return new(
                userFunction.FunctionId,
                userFunction.UserId,
                userFunction.Function.Participation.Id,
                userFunction.Function.Participation.Step.ProgrammingLanguage.Id,
                userFunction.LastModificationDate,
                userFunction.Function.Code,
                cheatingFunction.Rate,
                cheatingFunction.DetectionDate
            );
        }


        private static SuspectFunctionNavigation ToSuspectFunctionNavigation(UserFunction userFunction) =>
            new(
                userFunction.FunctionId,
                userFunction.UserId,
                userFunction.Function.Participation.Id,
                userFunction.Function.Participation.Step.ProgrammingLanguage.Id,
                userFunction.LastModificationDate,
                userFunction.Function.Code,
                userFunction.Function.PlagiarizedFunctions
                    .Where(f => f.IsValid == null)
                    .Select(f => f.PlagiarizedFunctionId)
                    .Distinct().ToList()
            );

        public async Task<IList<FunctionCodeNavigation>> GetAllNewFunctions()
        {
            var functions = await GetIncludeUserFunction()
                .Where(uF => !uF.Function.IsDeleted && !uF.User.IsDeleted)
                .GroupBy(uF => uF.FunctionId)
                .Select(userFuncGrp =>
                    userFuncGrp.OrderByDescending(function => function.LastModificationDate).First()
                )
                .ToListAsync();

            return functions
                .Select(ToFunctionCodeNavigation)
                .ToList();
        }

        public async Task<SuspectFunctionNavigation?> GetLastByFunctionId(Guid functionId)
        {
            var function = await GetIncludeUserFunction()
                .Where(uF => uF.FunctionId == functionId && !uF.Function.IsDeleted && !uF.User.IsDeleted)
                .OrderByDescending(uF => uF.LastModificationDate)
                .FirstOrDefaultAsync();
            return function is null ? null : ToSuspectFunctionNavigation(function);
        }
    }
}