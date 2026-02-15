using CrochetToysShop.Models.ViewModels.Toys;

namespace CrochetToysShop.Services.Interfaces
{
    public interface IToyService
    {
        Task<IReadOnlyCollection<ToyListItemViewModel>> GetAllAsync();
        Task<ToyDetailsViewModel?> GetDetailsAsync(int id);
    }
}
