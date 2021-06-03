using System;
using Domain.Contracts;
using Domain.Users;

namespace Domain.Participations
{
    public record FunctionId(Guid Value) : IEntityId
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class FunctionEntity : Entity<FunctionId>, IComparable<FunctionEntity>
    {
        public UserId UserId { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string Code { get; set; }
        public int? Order { get; set; }

        public FunctionEntity(FunctionId id, UserId userId, string code,DateTime lastModificationDate, int? order) : base(id)
        {
            UserId = userId;
            Code = code;
            Order = order;
            LastModificationDate = lastModificationDate;
        }

        public int CompareTo(FunctionEntity? other)
        {
            if (other?.Id == Id) return 0;
            if (other?.Order is null) return 1;
            if (Order is null) return -1;
            return Order.Value - other.Order.Value ;
        }
    }
}