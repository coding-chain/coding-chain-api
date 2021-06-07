using System.Text.RegularExpressions;

namespace CodingChainApi.Infrastructure.Services.Parser
{
    public class TypescriptCodeAnalyzer : ICodeAnalyzer
    {
        private string Code { get; set; }

        public string? GetInputType(string code)
        {
            Code = code;
            var startParamsIdx = GetStartParamsIdx();
            var endParamsIdx = GetEndParamsIdx(startParamsIdx);
            var parameters = GetParams(startParamsIdx, endParamsIdx);
            return GetInputParamsType(parameters).Trim();
        }

        public string? GetReturnType(string code)
        {
            Code = code;
            var startParamsIdx = GetStartParamsIdx();
            var endParamsIdx = GetEndParamsIdx(startParamsIdx);
            var endSignatureIdx = GetSignatureEndIdx();
            var afterParams = Code.Substring(endParamsIdx, endSignatureIdx - endParamsIdx);
            var outputsIdx = afterParams.IndexOf(':') + 1;
            return afterParams[outputsIdx..].Trim();
        }

        private int GetSignatureEndIdx()
        {
            var match = Regex.Match(Code, @"{[\s\S]*}");
            var endIdx = match.Length + match.Index;
            return CodeUtils.GetStartingScopeIdx(Code, endIdx - 1, '{', '}');
        }

        private int GetStartParamsIdx()
        {
            return Code.IndexOf('(');
        }


        private int GetEndParamsIdx(int startParamsIdx)
        {
            return CodeUtils.GetEndingScopeIdx(Code, startParamsIdx, '(', ')');
        }

        private string GetParams(int startParamsIdx, int endParamsIdx)
        {
            return Code.Substring(startParamsIdx, endParamsIdx - startParamsIdx);
        }

        private string GetInputParamsType(string parameters)
        {
            var startInputIdx = parameters.IndexOf(':') + 1;
            return parameters[startInputIdx..];
        }
    }
}