using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Languages;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using Domain.ProgrammingLanguages;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadProgrammingLanguageRepository : IReadProgrammingLanguageRepository
    {
        private readonly CodingChainContext _context;

        public ReadProgrammingLanguageRepository(CodingChainContext context)
        {
            _context = context;
        }

        public async Task<IPagedList<ProgrammingLanguageNavigation>> GetAllLanguageNavigationPaginated(
            PaginationQueryBase paginationQuery)
        {
            return await _context.ProgrammingLanguages
                .Select(l => new ProgrammingLanguageNavigation(l.Id, l.Name))
                .FromPaginationQueryAsync(paginationQuery);
        }


        public async Task<ProgrammingLanguageNavigation?> GetOneLanguageNavigationByIdAsync(Guid id)
        {
            var language = await _context.ProgrammingLanguages.FirstOrDefaultAsync(l => l.Id == id);
            if (language is null) return null;
            return new ProgrammingLanguageNavigation(language.Id, language.Name);
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