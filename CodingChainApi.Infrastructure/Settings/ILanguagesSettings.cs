using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Settings
{
    public interface ILanguagesSettings
    {
        public IList<string> AvailableLanguages { get; set; }
    }
}