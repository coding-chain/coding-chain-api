using System;

namespace Application.Common.Exceptions
{
    public class NotFoundException<TId, TSearchedItem> : ApplicationException
    {
        public NotFoundException(TId id)
        {
            Id = id;
            SearchedItemType = typeof(TSearchedItem);
        }

        public NotFoundException(string? message) : base(message)
        {
        }

        public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public TId Id { get; }
        public Type SearchedItemType { get; }

        public override string Message => $"Cannot find {SearchedItemType.FullName} with id {Id} ";
    }
}