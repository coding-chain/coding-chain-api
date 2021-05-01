using System.Threading.Tasks;
using Application.Contracts;
using Domain.Languages;
using Domain.ProgrammingLanguages;

namespace Application.Write.Contracts
{
    public interface IProgrammingLanguageRepository : IAggregateRepository<LanguageId, ProgrammingLanguage>
    {
        public Task<EnvironmentId> NextEnvironmentIdAsync();
    }
}