using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CodingChainApi.Infrastructure.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static Task<List<T>> ToMongoListAsync<T>(this IQueryable<T> mongoQueryOnly)
        {
            return ((IMongoQueryable<T>) mongoQueryOnly).ToListAsync();
        }
    }
}