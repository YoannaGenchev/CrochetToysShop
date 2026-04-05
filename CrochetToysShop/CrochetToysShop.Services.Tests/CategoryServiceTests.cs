namespace CrochetToysShop.Services.Tests
{
    using CrochetToysShop.Services.Core;
    using CrochetToysShop.Services.Tests.Infrastructure;
    using CrochetToysShop.Services.Tests.Infrastructure.Builders;
    using Xunit;

    public class CategoryServiceTests
    {
        [Fact]
        public async Task GetAllAsync_WithToysInMultipleCategories_ReturnsGroupedCategoryCounts()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Categories.Add(new CategoryBuilder().WithId(2).WithName("Animals").Build());

            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).Build());
            context.Toys.Add(new ToyBuilder().WithId(2).WithName("Tulip").WithCategoryId(1).Build());
            context.Toys.Add(new ToyBuilder().WithId(3).WithName("Fox").WithCategoryId(2).Build());
            context.SaveChanges();

            var service = new CategoryService(context);

            // Act
            var result = (await service.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Flowers" && c.ToyCount == 2);
            Assert.Contains(result, c => c.Name == "Animals" && c.ToyCount == 1);
        }

        [Fact]
        public async Task GetAllAsync_WithNoToys_ReturnsEmptyCollection()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Categories.Add(new CategoryBuilder().WithId(2).WithName("Animals").Build());
            context.SaveChanges();

            var service = new CategoryService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByNameAsync_WithExistingCategoryName_ReturnsMatchingToys()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Categories.Add(new CategoryBuilder().WithId(2).WithName("Animals").Build());

            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).Build());
            context.Toys.Add(new ToyBuilder().WithId(2).WithName("Tulip").WithCategoryId(1).Build());
            context.Toys.Add(new ToyBuilder().WithId(3).WithName("Fox").WithCategoryId(2).Build());
            context.SaveChanges();

            var service = new CategoryService(context);

            // Act
            var result = (await service.GetByNameAsync("  fLoWeRs  ")).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal("Flowers", t.CategoryName));
            Assert.DoesNotContain(result, t => t.Name == "Fox");
        }

        [Fact]
        public async Task GetByNameAsync_WithMissingCategoryName_ReturnsEmptyCollection()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).Build());
            context.SaveChanges();

            var service = new CategoryService(context);

            // Act
            var result = await service.GetByNameAsync("Seasonal");

            // Assert
            Assert.Empty(result);
        }
    }
}
