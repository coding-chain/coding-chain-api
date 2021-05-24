namespace Application.Common.Pagination
{
    public  record PaginationQueryBase
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}