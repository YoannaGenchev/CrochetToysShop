using CrochetToysShop.Data.Models;

namespace CrochetToysShop.Services.Tests.Infrastructure.Builders
{
    internal class CategoryBuilder
    {
        private readonly Category category = new()
        {
            Name = "Category"
        };

        public CategoryBuilder WithId(int id)
        {
            category.Id = id;
            return this;
        }

        public CategoryBuilder WithName(string name)
        {
            category.Name = name;
            return this;
        }

        public Category Build() => category;
    }
}
