using CrochetToysShop.Web.ViewModels.Toys;

namespace CrochetToysShop.Services.Core.Interfaces
{
    public interface IToyService
    {
        Task<ToyIndexViewModel> GetAllAsync(int? categoryId = null, int page = 1, int pageSize = 10);

        Task<ToyDetailsViewModel?> GetDetailsAsync(int id);

        Task<ToyFormViewModel> GetCreateModelAsync();

        Task CreateAsync(ToyFormViewModel model, string? userId = null);

        Task<ToyFormViewModel?> GetEditModelAsync(int id, string? userId = null);

        Task<bool> EditAsync(int id, ToyFormViewModel model, string? userId = null);

        Task<ToyDeleteViewModel?> GetDeleteModelAsync(int id, string? userId = null);

        Task<bool> DeleteAsync(int id, string? userId = null);
    }
}
