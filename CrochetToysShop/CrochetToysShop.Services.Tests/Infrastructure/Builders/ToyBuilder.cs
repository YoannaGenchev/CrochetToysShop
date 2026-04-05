using CrochetToysShop.Data.Models;

namespace CrochetToysShop.Services.Tests.Infrastructure.Builders
{
    internal class ToyBuilder
    {
        private readonly Toy toy = new()
        {
            Name = "Test Toy",
            Description = "Valid toy description with enough characters.",
            Price = 10.00m,
            ImageUrl = "/images/toys/test.jpg",
            SizeCm = 20,
            Difficulty = "Easy",
            IsAvailable = true,
            CategoryId = 1
        };

        public ToyBuilder WithId(int id)
        {
            toy.Id = id;
            return this;
        }

        public ToyBuilder WithName(string name)
        {
            toy.Name = name;
            return this;
        }

        public ToyBuilder WithDescription(string description)
        {
            toy.Description = description;
            return this;
        }

        public ToyBuilder WithCategoryId(int categoryId)
        {
            toy.CategoryId = categoryId;
            return this;
        }

        public ToyBuilder WithPrice(decimal price)
        {
            toy.Price = price;
            return this;
        }

        public ToyBuilder WithIsAvailable(bool isAvailable)
        {
            toy.IsAvailable = isAvailable;
            return this;
        }

        public ToyBuilder WithCreatedByUserId(string? userId)
        {
            toy.CreatedByUserId = userId;
            return this;
        }

        public Toy Build() => toy;
    }
}
