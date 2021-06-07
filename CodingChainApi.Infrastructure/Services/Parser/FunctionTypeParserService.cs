using System.Collections.Generic;
using Application.Read.Contracts;
using Domain.ProgrammingLanguages;

namespace CodingChainApi.Infrastructure.Services.Parser
{
    public class FunctionTypeParserService : IFunctionTypeParserService
    {
        private readonly Dictionary<LanguageEnum, ICodeAnalyzer> _codeAnalyzers = new()
        {
            {LanguageEnum.CSharp, new CsharpCodeAnalyzer()},
            {LanguageEnum.Typescript, new TypescriptCodeAnalyzer()},
        };

        public string? GetReturnType(string code, LanguageEnum language)
        {
            return _codeAnalyzers[language].GetReturnType(code);
        }

        public string? GetInputType(string code, LanguageEnum language)
        {
            return _codeAnalyzers[language].GetInputType(code);
        }
    }
}