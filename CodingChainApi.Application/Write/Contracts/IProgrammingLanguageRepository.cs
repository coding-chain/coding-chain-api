using Application.Contracts;
using Domain.ProgrammingLanguages;

namespace Application.Write.Contracts
{
    public interface IProgrammingLanguageRepository : IAggregateRepository<ProgrammingLanguageId, ProgrammingLanguage>
    {
    }
}