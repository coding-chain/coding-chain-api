using System;
using Domain.Users;

namespace Application.Common.Security
{
    /// <summary>
    ///     Specifies the class this attribute is applied to requires authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute
    {
        /// <summary>
        ///     Gets or sets a list of roles authorized for decorated class
        /// </summary>
        public RightEnum[] Roles { get; set; } = Array.Empty<RightEnum>();
    }
}