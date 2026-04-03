using CrochetToysShop.Web.ViewModels.Categories;
using CrochetToysShop.Web.ViewModels.Toys;

namespace CrochetToysShop.Services.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryListItemViewModel>> GetAllAsync();

        Task<IEnumerable<ToyListItemViewModel>> GetByNameAsync(string categoryName);
    }
}
