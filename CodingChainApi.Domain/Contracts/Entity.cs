using System.Collections.Generic;

namespace Domain.Contracts
{
    public class Entity<TId> where TId : IEntityId
    {
        public Entity(TId id)
        {
            Id = id;
        }

        public TId Id { get; set; }

        protected bool Equals(Entity<TId> other)
        {
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }


        public sealed override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity<TId>) obj);
        }

        public sealed override int GetHashCode()
        {
            return EqualityComparer<TId>.Default.GetHashCode(Id);
        }
    }
}