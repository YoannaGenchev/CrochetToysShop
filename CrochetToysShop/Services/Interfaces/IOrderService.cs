using CrochetToysShop.Models.ViewModels.Orders;

namespace CrochetToysShop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderCreateViewModel?> GetOrderModelAsync(int toyId);

        Task<(bool ok, string? error, int toyId)> CreateOrderAsync(OrderCreateViewModel model);

        Task<IEnumerable<OrderAdminListItemViewModel>> GetAllForAdminAsync();

        Task<bool> MarkCompletedAsync(int id);
    }
}
