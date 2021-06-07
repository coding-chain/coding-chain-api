using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.ProgrammingLanguages;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using Domain.ProgrammingLanguages;
using Microsoft.EntityFrameworkCore;
using ProgrammingLanguage = CodingChainApi.Infrastructure.Models.ProgrammingLanguage;

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
                .Where(l => !l.IsDeleted)
                .Select(l => new ProgrammingLanguageNavigation(l.Id, l.Name))
                .FromPaginationQueryAsync(paginationQuery);
        }

        public static ProgrammingLanguageNavigation ToProgrammingLanguageNavigation(ProgrammingLanguage language) =>
            new(
                language.Id, language.Name);

        public async Task<ProgrammingLanguageNavigation?> GetOneLanguageNavigationByIdAsync(Guid id)
        {
            var language = await _context.ProgrammingLanguages.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
            if (language is null) return null;
            return new ProgrammingLanguageNavigation(language.Id, language.Name);
        }


        public async Task<bool> LanguageExistById(Guid programmingLanguageId)
        {
            return await _context.ProgrammingLanguages.AnyAsync(
                l => !l.IsDeleted && l.Id == programmingLanguageId);
        }

        public async Task<bool> LanguageExistsByName(LanguageEnum name)
        {
            return await _context.ProgrammingLanguages.AnyAsync(
                l => !l.IsDeleted && l.Name == name);
        }
    }
}