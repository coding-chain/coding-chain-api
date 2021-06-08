using System;

namespace Application.Common.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string id, string searchedItem)
        {
            Id = id;
            SearchedItem = searchedItem;
        }

        public NotFoundException(string? message) : base(message)
        {
        }

        public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public string Id { get; }
        public string SearchedItem { get; }

        public override string Message => $"Cannot find {SearchedItem} with id {Id} ";
    }
}