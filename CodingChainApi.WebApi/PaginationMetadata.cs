using Application.Common.Pagination;

namespace NeosCodingApi
{
    public class PaginationMetadata
    {
        public long CurrentPage { get; private set; }
        public long TotalPages { get; private set; }
        public long PageSize { get; private set; }
        public long TotalCount { get; private set; }


        public static PaginationMetadata Create<T>(IPagedList<T> pagedList)
        {
            var metadata = new PaginationMetadata
            {
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                PageSize = pagedList.PageSize,
                TotalCount = pagedList.TotalCount
            };
            return metadata;
        }
    }
}