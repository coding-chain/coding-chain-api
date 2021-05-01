using System;
using System.Collections.Generic;
using Application.Common.Pagination;

namespace CodingChainApi.Infrastructure.Common.Pagination
{
    public class PagedList<T>: List<T>, IPagedList<T>
    {
        public long CurrentPage { get; }
        public long TotalPages { get; }
        public long PageSize { get; }
        public long TotalCount { get; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        private PagedList(IList<T> items, long count, long pageNumber, long pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (long)Math.Ceiling(count / (double) pageSize);
            AddRange(items);
        }

        public static  PagedList<T> FromPaginationQuery(IList<T> items, long count, IPaginationQuery query)
        {
            return new(items, count, query.Page, query.Size);
        }
    }
}