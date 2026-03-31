using CrochetToysShop.Web.ViewModels.Toys;

namespace CrochetToysShop.Services.Core.Interfaces
{
    public interface IToyService
    {
        Task<ToyIndexViewModel> GetAllAsync(int? categoryId = null);

        Task<ToyDetailsViewModel?> GetDetailsAsync(int id);

        Task<ToyFormViewModel> GetCreateModelAsync();

        Task CreateAsync(ToyFormViewModel model);

        Task<ToyFormViewModel?> GetEditModelAsync(int id);

        Task<bool> EditAsync(int id, ToyFormViewModel model);

        Task<ToyDeleteViewModel?> GetDeleteModelAsync(int id);

        Task<bool> DeleteAsync(int id);
    }
}
