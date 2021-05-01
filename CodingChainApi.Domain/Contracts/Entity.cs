namespace Domain.Contracts
{
    public class Entity<TId> where TId: IEntityId
    {
        public Entity(TId id)
        {
            Id = id;
        }

        public TId Id { get; set; }
    }
}