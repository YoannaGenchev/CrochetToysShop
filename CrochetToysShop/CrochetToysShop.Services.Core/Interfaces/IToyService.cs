using CrochetToysShop.Web.ViewModels.Toys;

namespace CrochetToysShop.Services.Core.Interfaces
{
    public interface IToyService
    {
        Task<ToyIndexViewModel> GetAllAsync(int? categoryId = null, int page = 1, int pageSize = 10);

        Task<ToyIndexViewModel> SearchAsync(string? searchTerm = null, int? categoryId = null, int page = 1, int pageSize = 10);

        Task<ToyIndexViewModel> GetByCategoryNameAsync(string categoryName);

        Task<ToyDetailsViewModel?> GetDetailsAsync(int id);

        Task<ToyFormViewModel> GetCreateModelAsync();

        Task<int> CreateAsync(ToyFormViewModel model, string? userId = null);

        Task<ToyFormViewModel?> GetEditModelAsync(int id, string? userId = null, bool isAdmin = false);

        Task<bool> EditAsync(int id, ToyFormViewModel model, string? userId = null, bool isAdmin = false);

        Task<ToyDeleteViewModel?> GetDeleteModelAsync(int id, string? userId = null, bool isAdmin = false);

        Task<bool> DeleteAsync(int id, string? userId = null, bool isAdmin = false);
    }
}
