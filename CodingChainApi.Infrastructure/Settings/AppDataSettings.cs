namespace CodingChainApi.Infrastructure.Settings
{
    public interface IAppDataSettings
    {
        string BasePath { get; set; }
        string TemplatesPath { get; set; }
        string TournamentsPath { get; set; }
    }
    public class AppDataSettings : IAppDataSettings
    {
        public string BasePath { get; set; }
        public string TemplatesPath { get; set; }
        public string TournamentsPath { get; set; }
    }
}