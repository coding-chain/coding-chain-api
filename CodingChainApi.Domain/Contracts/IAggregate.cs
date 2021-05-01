using System.Collections.Generic;

namespace Domain.Contracts
{
    public interface IAggregate
    {
        public void ClearEvents();
        public IReadOnlyList<IDomainEvent> Events { get; }
    }
}