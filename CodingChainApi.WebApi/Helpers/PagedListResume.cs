using Application.Common.Pagination;

namespace NeosCodingApi.Helpers
{
    public static class IPageListExtensions
    {
        public static PagedListResume ToPagedListResume<T>(this IPagedList<T> page)
        {
            return new(page.CurrentPage, page.TotalPages, page.PageSize, page.TotalCount,
                page.HasPrevious, page.HasNext);
        }
    }

    public class PagedListResume
    {
        public PagedListResume(long currentPage, long totalPages, long pageSize, long totalCount, bool hasPrevious,
            bool hasNext)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            PageSize = pageSize;
            TotalCount = totalCount;
            HasPrevious = hasPrevious;
            HasNext = hasNext;
        }

        public long CurrentPage { get; }
        public long TotalPages { get; }
        public long PageSize { get; }
        public long TotalCount { get; }
        public bool HasPrevious { get; }
        public bool HasNext { get; }
    }
}