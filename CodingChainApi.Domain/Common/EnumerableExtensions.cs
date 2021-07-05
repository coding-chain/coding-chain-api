using System.Collections.Generic;
using System.Linq;

namespace Domain.Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> RemoveNull<T>(this IEnumerable<T?> values)
        {
            return values.Where(v => v is not null)!;
        }

        public static T[] RemoveNull<T>(this T?[] values)
        {
            return values.Where(v => v is not null).ToArray()!;
        }
    }
}