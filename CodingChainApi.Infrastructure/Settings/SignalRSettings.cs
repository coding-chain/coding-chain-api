namespace CodingChainApi.Infrastructure.Settings
{
    public interface ISignalRSettings
    {
        string? ConnectionString { get; set; }
    }

    public class SignalRSettings : ISignalRSettings
    {
        public string? ConnectionString { get; set; }
    }
}