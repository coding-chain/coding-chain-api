using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using CodingChainApi.Infrastructure.Common.Pagination;
using Microsoft.EntityFrameworkCore;

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

        public static PagedList<T> FromEnumerable<T>(this IEnumerable<T> enumerable,
            PaginationQueryBase pageQuery)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();
            var count = array.Length;
            var skip = (pageQuery.Page - 1) * pageQuery.Size;
            var items = array.Skip(skip).Take(pageQuery.Size);
            return new PagedList<T>(items.ToList(), count, pageQuery.Page, pageQuery.Size);
        }
    }
}