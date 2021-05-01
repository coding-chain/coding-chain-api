namespace CodingChainApi.Infrastructure.Settings
{
    public interface IAppDataSettings
    {
        string BasePath { get; set; }
        string TemplatesPath { get; set; }
    }
}