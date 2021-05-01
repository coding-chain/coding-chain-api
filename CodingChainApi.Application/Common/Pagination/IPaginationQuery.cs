namespace Application.Common.Pagination
{
    public interface IPaginationQuery
    {
        public int Page { get; }
        public int Size { get; }
    }

}