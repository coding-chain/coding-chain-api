namespace Domain.Users
{
    public record Right(RightEnum Name)
    {
        public RightEnum Name { get; init; } = Name;
    }
}