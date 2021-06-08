using System.Collections.Generic;

namespace Domain.Contracts
{
    public interface IAggregate
    {
        public IReadOnlyList<IDomainEvent> Events { get; }
        public void ClearEvents();
    }
}