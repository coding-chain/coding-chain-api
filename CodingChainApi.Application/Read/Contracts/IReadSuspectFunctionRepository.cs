using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Plagiarism;
using Application.Read.Plagiarism.Handlers;

namespace Application.Read.Contracts
{
    public record GetFunctionsQuery : PaginationQueryBase
    {
        public DateTime? LowerThanDateFilter { get; set; }
        public DateTime? GreaterThanDateFilter { get; set; }
        public Guid? ExcludedUserIdFilter { get; set; }
        public IList<Guid>? FunctionsIdsFilter { get; set; }
        public Guid? LanguageIdFilter { get; set; }
    }

    public interface IReadSuspectFunctionRepository
    {
        public Task<IList<SuspectFunctionNavigation>> GetAllLastFunctionFiltered(GetFunctionsQuery query);

        public Task<IPagedList<PlagiarizedFunctionNavigation>> GetPlagiarizedFunctions(
            GetLastPlagiarizedFunctionsByFunctionQuery query);

        public Task<IPagedList<SuspectFunctionNavigation>> GetPaginatedLastSuspectFunctionsFiltered(
            GetFunctionsQuery query);

        public Task<SuspectFunctionNavigation?> GetLastByFunctionId(Guid functionId);
        
    }
}