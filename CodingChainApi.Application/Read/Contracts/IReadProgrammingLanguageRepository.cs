using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Languages;
using Domain.Languages;
using Domain.ProgrammingLanguages;

namespace Application.Read.Contracts
{
    public interface IReadProgrammingLanguageRepository
    {
        public Task<IPagedList<ProgrammingLanguageNavigation>> GetAllLanguageNavigationPaginated(IPaginationQuery paginationQuery);
        public Task<ProgrammingLanguageNavigation?> GetOneLanguageNavigationByIdAsync(LanguageId id);
        public Task<bool> LanguageExistById(LanguageId languageId);
        public Task<bool> LanguageExistsByName(string name);
    }
}