namespace Motoflow.Models.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public Dictionary<string, string> Links { get; set; } = [];

        public PagedResult(IEnumerable<T> data, int page, int pageSize, int totalCount)
        {
            Data = data;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            HasPreviousPage = page > 1;
            HasNextPage = page < TotalPages;
        }
    }

    public class PaginationQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public PaginationQuery()
        {
            Page = Page < 1 ? 1 : Page;
            PageSize = PageSize > 100 ? 100 : PageSize < 1 ? 10 : PageSize;
        }
    }
}