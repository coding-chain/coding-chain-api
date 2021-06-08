using System;
using System.Threading.Tasks;
using Application.Write.Contracts;
using Domain.ProgrammingLanguages;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class ProgrammingLanguageRepository : IProgrammingLanguageRepository
    {
        public Task<ProgrammingLanguageId> SetAsync(ProgrammingLanguage aggregate)
        {
            throw new NotImplementedException();
        }

        public Task<ProgrammingLanguage?> FindByIdAsync(ProgrammingLanguageId id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(ProgrammingLanguageId id)
        {
            throw new NotImplementedException();
        }

        public Task<ProgrammingLanguageId> NextIdAsync()
        {
            throw new NotImplementedException();
        }
    }
}