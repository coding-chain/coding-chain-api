using System.Threading.Tasks;

namespace CodingChainApi.Infrastructure.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static Task<T> ToTask<T>(this T taskResult)
        {
            return Task.FromResult(taskResult);
        }
    }
}