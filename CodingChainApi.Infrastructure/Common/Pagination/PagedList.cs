using System;
using System.Collections.Generic;
using System.Linq;
using Application.Common.Pagination;

namespace CodingChainApi.Infrastructure.Common.Pagination
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public long CurrentPage { get; }
        public long TotalPages { get; }
        public long PageSize { get; }
        public long TotalCount { get; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(IList<T> items, long count, long pageNumber, long pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (long) Math.Ceiling(count / (double) pageSize);
            AddRange(items);
        }

        public static PagedList<T> Empty(long pageNumber, long pageSize)
        {
            return new(new List<T>(), 0, pageNumber, pageSize);
        }

        public static PagedList<T> From<U>(IList<T> items, PagedList<U> page)
        {
            return new PagedList<T>(items, page.Count, page.CurrentPage, page.PageSize);
        }
    }
}