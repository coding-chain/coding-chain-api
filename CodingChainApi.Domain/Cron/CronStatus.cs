namespace Domain.Cron
{
    public record CronStatus(CronStatusEnum Status)
    {
        public CronStatusEnum Status { get; set; } = Status;
    }
}