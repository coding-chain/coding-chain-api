namespace CodingChainApi.Infrastructure.Settings
{
    public interface IDatabaseSettings
    {
        string? ConnectionString { get; set; }
    }
}