namespace CodingChainApi.Infrastructure.Settings
{
    public interface ICorsSettings
    {
        string FrontEndUrl { get; set; }
    }

    public class CorsSettings : ICorsSettings
    {
        public string FrontEndUrl { get; set; }
    }
}