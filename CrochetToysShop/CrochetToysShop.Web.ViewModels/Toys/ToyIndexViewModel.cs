namespace CrochetToysShop.Web.ViewModels.Toys
{
    using CrochetToysShop.Web.ViewModels.Common;

    public class ToyIndexViewModel
    {
        public IEnumerable<ToyListItemViewModel> Toys { get; set; } = new List<ToyListItemViewModel>();

        public int? CategoryId { get; set; }

        public IEnumerable<CategoryDropdownViewModel> Categories { get; set; } = new List<CategoryDropdownViewModel>();

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
