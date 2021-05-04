using System.Threading.Tasks;
using Application.Write.Contracts;
using Domain.ProgrammingLanguages;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class ProgrammingLanguageRepository : IProgrammingLanguageRepository
    {
        private IProgrammingLanguageRepository _programmingLanguageRepositoryImplementation;
        public Task<LanguageId> SetAsync(ProgrammingLanguage aggregate)
        {
            return _programmingLanguageRepositoryImplementation.SetAsync(aggregate);
        }

        public Task<ProgrammingLanguage?> FindByIdAsync(LanguageId id)
        {
            return _programmingLanguageRepositoryImplementation.FindByIdAsync(id);
        }

        public Task RemoveAsync(LanguageId id)
        {
            return _programmingLanguageRepositoryImplementation.RemoveAsync(id);
        }

        public Task<LanguageId> NextIdAsync()
        {
            return _programmingLanguageRepositoryImplementation.NextIdAsync();
        }

    }
}