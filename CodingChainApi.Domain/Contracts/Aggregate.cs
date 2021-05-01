using System.Collections.Generic;

namespace Domain.Contracts
{
    public abstract class Aggregate<TId>: Entity<TId>, IAggregate where TId : IEntityId
    {
        public IReadOnlyList<IDomainEvent> Events => _events.AsReadOnly();

        public Aggregate(TId id) : base(id)
        {
        }

        private readonly List<IDomainEvent> _events = new();

        protected Aggregate<TId> RegisterEvent(IDomainEvent domainEvent)
        {
            _events.Add(domainEvent);
            return this;
        }

        public void ClearEvents()
        {
            _events.Clear();
        }
    }
}