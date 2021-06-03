using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodingChainApi.Infrastructure.Services.Parser
{
    public class CsharpCodeAnalyzer
    {
        public string? GetInputType(string code)
        {
            var functionName = FindFunctionName(code);
            if (functionName is null)
            {
                return null;
            }

            var parameters = code.Substring(StartParamIdx, EndParamIdx - StartParamIdx).Trim();
            parameters = parameters.Substring(1, parameters.Length - 1);
            var split = parameters.Split(' ').ToList();
            split.RemoveAt(split.Count - 1);
            return string.Join(null, split).Trim();
        }

        public string? GetReturnType(string code)
        {
            var functionName = FindFunctionName(code);
            if (functionName is null)
            {
                return null;
            }

            var startIdx = code.IndexOf("static", StringComparison.Ordinal) + "static".Length;
            return code.Substring(startIdx, code.IndexOf(functionName, StringComparison.Ordinal) - startIdx).Trim();
        }

        public string? FindFunctionName(string code)
        {
            var match = Regex.Match(code, @"{[\s\S]*}|=>");
            var openBracketCnt = 0;
            var hasChanged = false;
            int i;
            EndParamIdx = match.Index;
            for (i = match.Index; i >= 0; i--)
            {
                if (code[i] == ')')
                {
                    hasChanged = true;
                    openBracketCnt--;
                }

                if (code[i] == '(')
                {
                    hasChanged = true;
                    openBracketCnt++;
                }

                if (openBracketCnt == 0 && hasChanged) break;
            }

            code = code[..i];
            var nameMatch = Regex.Match(code, @"\b(\w+)$");
            StartParamIdx = nameMatch.Index + nameMatch.Value.Length;
            return nameMatch.Value;
        }

        public int StartParamIdx { get; set; }

        public int EndParamIdx { get; set; }
    }
}