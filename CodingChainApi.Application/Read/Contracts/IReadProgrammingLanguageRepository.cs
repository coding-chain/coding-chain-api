using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.ProgrammingLanguages;
using Domain.ProgrammingLanguages;

namespace Application.Read.Contracts
{
    public interface IReadProgrammingLanguageRepository
    {
        public Task<IPagedList<ProgrammingLanguageNavigation>> GetAllLanguageNavigationPaginated(PaginationQueryBase paginationQuery);
        public Task<ProgrammingLanguageNavigation?> GetOneLanguageNavigationByIdAsync(Guid id);
        public Task<bool> LanguageExistById(Guid programmingLanguageId);
        public Task<bool> LanguageExistsByName(LanguageEnum name);


    }
}