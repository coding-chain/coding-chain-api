namespace CodingChainApi.Infrastructure.Services.Parser
{
    public static class CodeUtils
    {
        public static int GetStartingScopeIdx(string text, int endScopeIdx, char startScopeChar, char endScopeChar)
        {
            var startScopeCharCnt = 0;
            var hasChanged = false;
            for (var i = endScopeIdx; i >= 0; i--)
            {
                if (text[i] == endScopeChar)
                {
                    hasChanged = true;
                    startScopeCharCnt--;
                }
                else if (text[i] == startScopeChar)
                {
                    hasChanged = true;
                    startScopeCharCnt++;
                }

                if (startScopeCharCnt == 0 && hasChanged) return i;
            }

            return -1;
        }

        public static int GetEndingScopeIdx(string text, int startScopeIdx, char startScopeChar, char endScopeChar)
        {
            var endScopeCharCnt = 0;
            var hasChanged = false;
            for (var i = startScopeIdx; i < text.Length; i++)
            {
                if (text[i] == endScopeChar)
                {
                    hasChanged = true;
                    endScopeCharCnt++;
                }
                else if (text[i] == startScopeChar)
                {
                    hasChanged = true;
                    endScopeCharCnt--;
                }

                if (endScopeCharCnt == 0 && hasChanged) return i;
            }

            return -1;
        }
    }
}