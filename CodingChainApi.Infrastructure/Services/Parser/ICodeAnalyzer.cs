namespace CodingChainApi.Infrastructure.Services.Parser
{
    public interface ICodeAnalyzer
    {
        string? GetInputType(string code);
        string? GetReturnType(string code);
    }
}