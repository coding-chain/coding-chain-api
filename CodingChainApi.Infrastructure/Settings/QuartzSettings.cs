namespace CodingChainApi.Infrastructure.Settings
{
    public interface IQuartzSettings
    {
        public string PlagiarismAnalysisCronJob { get; set; }
    }

    public class QuartzSettings: IQuartzSettings
    {
        public string PlagiarismAnalysisCronJob { get; set; }
    }
}