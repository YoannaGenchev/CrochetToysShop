namespace CrochetToysShop.Services.Tests
{
    using CrochetToysShop.Data;
    using CrochetToysShop.Data.Models;
    using CrochetToysShop.Services.Core;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ToyServiceTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_WithNoFilter_ReturnsAllToys()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var category = new Category { Id = 1, Name = "Flowers" };
                context.Categories.Add(category);
                context.Toys.Add(new Toy { Id = 1, Name = "Rose", CategoryId = 1, Price = 10.00m, IsAvailable = true });
                context.Toys.Add(new Toy { Id = 2, Name = "Tulip", CategoryId = 1, Price = 12.00m, IsAvailable = true });
                context.SaveChanges();

                var service = new ToyService(context);

                // Act
                var result = await service.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Toys.Count());
            }
        }

        [Fact]
        public async Task GetAllAsync_WithCategoryFilter_ReturnsFilturedToys()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                context.Categories.Add(new Category { Id = 1, Name = "Flowers" });
                context.Categories.Add(new Category { Id = 2, Name = "Animals" });
                context.Toys.Add(new Toy { Id = 1, Name = "Rose", CategoryId = 1, Price = 10.00m, IsAvailable = true });
                context.Toys.Add(new Toy { Id = 2, Name = "Dog", CategoryId = 2, Price = 15.00m, IsAvailable = true });
                context.SaveChanges();

                var service = new ToyService(context);

                // Act
                var result = await service.GetAllAsync(categoryId: 1);

                // Assert
                Assert.NotNull(result);
                Assert.Single(result.Toys);
                Assert.Equal("Rose", result.Toys.First().Name);
            }
        }

        [Fact]
        public async Task GetAllAsync_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var category = new Category { Id = 1, Name = "General" };
                context.Categories.Add(category);
                for (int i = 1; i <= 25; i++)
                {
                    context.Toys.Add(new Toy
                    {
                        Id = i,
                        Name = $"Toy{i}",
                        CategoryId = 1,
                        Price = 10.00m + i,
                        IsAvailable = true
                    });
                }
                context.SaveChanges();

                var service = new ToyService(context);

                // Act
                var result = await service.GetAllAsync(page: 2, pageSize: 10);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(10, result.Toys.Count());
                Assert.Equal(3, result.Pagination.TotalPages);
                Assert.Equal(2, result.Pagination.CurrentPage);
                Assert.Equal(25, result.Pagination.TotalCount);
            }
        }

        [Fact]
        public async Task GetDetailsAsync_WithValidId_ReturnsCorrectToy()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var category = new Category { Id = 1, Name = "Flowers" };
                context.Categories.Add(category);
                var toy = new Toy
                {
                    Id = 1,
                    Name = "Rose",
                    Description = "Beautiful red rose",
                    CategoryId = 1,
                    Price = 10.00m,
                    IsAvailable = true,
                    SizeCm = 15,
                    Difficulty = "Easy"
                };
                context.Toys.Add(toy);
                context.SaveChanges();

                var service = new ToyService(context);

                // Act
                var result = await service.GetDetailsAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Rose", result.Name);
                Assert.Equal("Beautiful red rose", result.Description);
            }
        }

        [Fact]
        public async Task GetDetailsAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var service = new ToyService(context);

                // Act
                var result = await service.GetDetailsAsync(999);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesToy()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var category = new Category { Id = 1, Name = "Flowers" };
                context.Categories.Add(category);
                context.SaveChanges();

                var service = new ToyService(context);
                var model = new CrochetToysShop.Web.ViewModels.Toys.ToyFormViewModel
                {
                    Name = "New Rose",
                    Description = "Pink rose",
                    CategoryId = 1,
                    Price = 12.00m,
                    IsAvailable = true,
                    SizeCm = 16,
                    Difficulty = "Easy"
                };

                // Act
                await service.CreateAsync(model, "user123");

                // Assert
                var createdToy = await context.Toys.FirstOrDefaultAsync(t => t.Name == "New Rose");
                Assert.NotNull(createdToy);
                Assert.Equal("user123", createdToy.CreatedByUserId);
            }
        }

        [Fact]
        public async Task EditAsync_WithValidData_UpdatesToy()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var category = new Category { Id = 1, Name = "Flowers" };
                context.Categories.Add(category);
                var toy = new Toy
                {
                    Id = 1,
                    Name = "Old Name",
                    CategoryId = 1,
                    Price = 10.00m,
                    IsAvailable = true,
                    CreatedByUserId = "user123"
                };
                context.Toys.Add(toy);
                context.SaveChanges();

                var service = new ToyService(context);
                var updateModel = new CrochetToysShop.Web.ViewModels.Toys.ToyFormViewModel
                {
                    Name = "Updated Rose",
                    Description = "Updated description",
                    CategoryId = 1,
                    Price = 15.00m,
                    IsAvailable = true,
                    SizeCm = 18,
                    Difficulty = "Medium"
                };

                // Act
                var result = await service.EditAsync(1, updateModel, "user123");

                // Assert
                Assert.True(result);
                var updated = await context.Toys.FindAsync(1);
                Assert.Equal("Updated Rose", updated.Name);
            }
        }

        [Fact]
        public async Task EditAsync_WithUnauthorizedUser_ReturnsFalse()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var toy = new Toy
                {
                    Id = 1,
                    Name = "Old Name",
                    CategoryId = 1,
                    CreatedByUserId = "otherUser"
                };
                context.Toys.Add(toy);
                context.SaveChanges();

                var service = new ToyService(context);
                var updateModel = new CrochetToysShop.Web.ViewModels.Toys.ToyFormViewModel
                {
                    Name = "Updated Rose",
                    CategoryId = 1,
                    Price = 15.00m
                };

                // Act
                var result = await service.EditAsync(1, updateModel, "user123");

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task DeleteAsync_WithValidOwnership_DeletesToy()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var toy = new Toy
                {
                    Id = 1,
                    Name = "Rose",
                    CategoryId = 1,
                    CreatedByUserId = "user123"
                };
                context.Toys.Add(toy);
                context.SaveChanges();

                var service = new ToyService(context);

                // Act
                var result = await service.DeleteAsync(1, "user123");

                // Assert
                Assert.True(result);
                var deleted = await context.Toys.FindAsync(1);
                Assert.Null(deleted);
            }
        }

        [Fact]
        public async Task DeleteAsync_WithNonexistentToy_ReturnsFalse()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var service = new ToyService(context);

                // Act
                var result = await service.DeleteAsync(999, "user123");

                // Assert
                Assert.False(result);
            }
        }
    }
}
