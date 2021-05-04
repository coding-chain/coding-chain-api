using System;
using Domain.Users;

namespace Application.Common.Security
{
    /// <summary>
    ///     Specifies the class this attribute is applied to requires authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AuthenticatedAttribute : Attribute
    {
    }
}