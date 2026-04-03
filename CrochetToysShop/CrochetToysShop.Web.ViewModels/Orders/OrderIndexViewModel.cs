namespace CrochetToysShop.Web.ViewModels.Orders
{
    using CrochetToysShop.Web.ViewModels.Common;

    public class OrderIndexViewModel
    {
        public IEnumerable<OrderAdminListItemViewModel> Orders { get; set; } = new List<OrderAdminListItemViewModel>();

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
