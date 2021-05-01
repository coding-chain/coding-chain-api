namespace CodingChainApi.Infrastructure.Settings
{
    public class AppDataSettings : IAppDataSettings
    {
        public string BasePath { get; set; }
        public string TemplatesPath { get; set; }
    }
}