namespace CrochetToysShop.Web.ViewModels.Common
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        public int PageSize { get; set; } = 10;

        public int TotalCount { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;

        public IEnumerable<int> PageNumbers
        {
            get
            {
                var pages = new List<int>();
                for (int i = 1; i <= TotalPages; i++)
                {
                    pages.Add(i);
                }
                return pages;
            }
        }
    }
}
