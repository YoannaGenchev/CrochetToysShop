using CrochetToysShop.Models.ViewModels.Toys;

namespace CrochetToysShop.Services.Interfaces
{
    public interface IToyService
    {
        Task<IReadOnlyCollection<ToyListItemViewModel>> GetAllAsync();
        Task<ToyDetailsViewModel?> GetDetailsAsync(int id);
        Task<ToyFormViewModel> GetCreateModelAsync();
        Task CreateAsync(ToyFormViewModel model);
        Task<ToyFormViewModel?> GetEditModelAsync(int id);
        Task<bool> EditAsync(int id, ToyFormViewModel model);
        Task<ToyDeleteViewModel?> GetDeleteModelAsync(int id);
        Task<bool> DeleteAsync(int id);


    }
}
