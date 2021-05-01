using System;
using System.Collections.Generic;

namespace Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(IList<string> errors) : base(string.Join(Environment.NewLine, errors))
        {
            Errors = errors;
        }

        public DomainException(string? message) : base(message)
        {
        }

        public DomainException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public IList<string> Errors { get; } = new List<string>();
    }
}