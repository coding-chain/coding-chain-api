using System.Collections.Generic;

namespace CodingChainApi.Infrastructure.Settings
{
    public interface IProcessSettings
    {
        public Dictionary<string, string> OsCommandLineMapping { get; set; }
    }

    public class ProcessSettings : IProcessSettings
    {
        public ProcessSettings(Dictionary<string, string> osCommandLineMapping)
        {
            OsCommandLineMapping = osCommandLineMapping;
        }

        public Dictionary<string, string> OsCommandLineMapping { get; set; }
    }
}