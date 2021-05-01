using System;

namespace Application.Common.Exceptions
{
    public class UnauthorizedAccessException : ApplicationException
    {
        public UnauthorizedAccessException(string? message) : base(message)
        {
        }

        public UnauthorizedAccessException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}