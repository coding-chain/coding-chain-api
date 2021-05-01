using System.Collections.Generic;
using System.Linq;

namespace CodingChainApi.Infrastructure.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WithoutNullValues<T>(this IEnumerable<T?> values)
        {
            return values.Where(v => v is not null)!;
        }

        public static T[] WithoutNullValues<T>(this T?[] values)
        {
            return values.Where(v => v is not null).ToArray()!;
        }
    }
}