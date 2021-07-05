using Domain.ProgrammingLanguages;

namespace Application.Read.Contracts
{
    public interface IFunctionTypeParserService
    {
        string? GetReturnType(string code, LanguageEnum language);
        string? GetInputType(string code, LanguageEnum language);
    }
}