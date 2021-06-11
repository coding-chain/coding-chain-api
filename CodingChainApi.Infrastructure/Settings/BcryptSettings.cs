namespace CodingChainApi.Infrastructure.Settings
{
    public interface IBcryptSettings
    {
        int WorkFactor { get; set; }
    }
    public class BcryptSettings : IBcryptSettings
    {
        public int WorkFactor { get; set; }
    }
}