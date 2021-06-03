using Application.Read.Contracts;
using Domain.ProgrammingLanguages;

namespace CodingChainApi.Infrastructure.Services.Parser
{
    public class FunctionTypeParserService : IFunctionTypeParserService
    {
        public string? GetReturnType(string code, LanguageEnum language)
        {
            return new CsharpCodeAnalyzer().GetReturnType(code);
        }

        public string? GetInputType(string code, LanguageEnum language)
        {
            return new CsharpCodeAnalyzer().GetInputType(code);
        }
    }
}