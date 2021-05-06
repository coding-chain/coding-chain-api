using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Common.Extensions
{
    public static class DbSetExtensions
    {
        public static void Upsert<T>(this DbSet<T> set, T entity ) where T : class
        {
            if (set.Contains(entity))
                set.Update(entity);
            else set.Add(entity);
        }
    }
}