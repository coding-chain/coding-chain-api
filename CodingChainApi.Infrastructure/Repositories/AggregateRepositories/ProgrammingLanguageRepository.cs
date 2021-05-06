using System.Threading.Tasks;
using Application.Write.Contracts;
using Domain.ProgrammingLanguages;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class ProgrammingLanguageRepository : IProgrammingLanguageRepository
    {
        public Task<ProgrammingLanguageId> SetAsync(ProgrammingLanguage aggregate)
        {
            throw new System.NotImplementedException();
        }

        public Task<ProgrammingLanguage?> FindByIdAsync(ProgrammingLanguageId id)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveAsync(ProgrammingLanguageId id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ProgrammingLanguageId> NextIdAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}