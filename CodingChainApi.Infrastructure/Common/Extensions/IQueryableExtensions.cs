using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using CodingChainApi.Infrastructure.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedList<T>> FromPaginationQueryAsync<T>(this IQueryable<T> query, PaginationQueryBase pageQuery)
        {
            var count =  query.Count();
            var skip = (pageQuery.Page - 1) * pageQuery.Size;
            var items =   await query.Skip(skip).Take(pageQuery.Size).ToListAsync();
            return new PagedList<T>(items, count, pageQuery.Page, pageQuery.Size);
        }
    }
}