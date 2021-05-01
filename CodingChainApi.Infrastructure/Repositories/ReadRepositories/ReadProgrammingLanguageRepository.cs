using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Languages;
using Domain.Languages;
using Domain.ProgrammingLanguages;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadProgrammingLanguageRepository : IReadProgrammingLanguageRepository
    
    {
        public Task<IPagedList<ProgrammingLanguageNavigation>> GetAllLanguageNavigationPaginated(IPaginationQuery paginationQuery)
        {
            throw new NotImplementedException();
        }



        public Task<ProgrammingLanguageNavigation?> GetOneLanguageNavigationByIdAsync(LanguageId id)
        {
            throw new NotImplementedException();
        }
        

        public Task<bool> LanguageExistById(LanguageId languageId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LanguageExistsByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}