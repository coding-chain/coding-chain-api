using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Contracts.Dtos;
using Application.Read.Functions;

namespace Application.Read.Contracts
{
    public interface IReadFunctionRepository
    {
        public Task<IPagedList<FunctionNavigation>> GetAllFunctionsPaginated(PaginationQueryBase paginationQueryBase);
        public Task<FunctionNavigation?> GetOneFunctionNavigationByIdAsync(Guid functionId);
        public Task<IList<Function>> GetAllFunctionFilterOnModifiedDate(DateTime dateFilter);
        public Task<IList<Function>> GetAllFunctionsNotPaginated();
    }
}