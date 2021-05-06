using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Settings
{
    public class LanguagesSettings:ILanguagesSettings
    {
        public IList<string> AvailableLanguages { get; set; } = new List<string>();
    }
}