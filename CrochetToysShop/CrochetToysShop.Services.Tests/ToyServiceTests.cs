namespace CrochetToysShop.Services.Tests
{
    using CrochetToysShop.Services.Core;
    using CrochetToysShop.Services.Tests.Infrastructure;
    using CrochetToysShop.Services.Tests.Infrastructure.Builders;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ToyServiceTests
    {
        [Fact]
        public async Task GetAllAsync_WithNoFilter_ReturnsAllToys()
        {
            // Arrange
            using (var context = TestDbContextFactory.Create())
            {
                var category = new CategoryBuilder().WithId(1).WithName("Flowers").Build();
                context.Categories.Add(category);
                context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithPrice(10.00m).WithIsAvailable(true).Build());
                context.Toys.Add(new ToyBuilder().WithId(2).WithName("Tulip").WithCategoryId(1).WithPrice(12.00m).WithIsAvailable(true).Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                context.Categories.Add(new CategoryBuilder().WithId(2).WithName("Animals").Build());
                context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithPrice(10.00m).WithIsAvailable(true).Build());
                context.Toys.Add(new ToyBuilder().WithId(2).WithName("Dog").WithCategoryId(2).WithPrice(15.00m).WithIsAvailable(true).Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                var category = new CategoryBuilder().WithId(1).WithName("General").Build();
                context.Categories.Add(category);
                for (int i = 1; i <= 25; i++)
                {
                    context.Toys.Add(
                        new ToyBuilder()
                            .WithId(i)
                            .WithName($"Toy{i}")
                            .WithCategoryId(1)
                            .WithPrice(10.00m + i)
                            .WithIsAvailable(true)
                            .Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                var category = new CategoryBuilder().WithId(1).WithName("Flowers").Build();
                context.Categories.Add(category);
                var toy = new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithDescription("Beautiful red rose")
                    .WithCategoryId(1)
                    .WithPrice(10.00m)
                    .WithIsAvailable(true)
                    .Build();
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
            using (var context = TestDbContextFactory.Create())
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
            using (var context = TestDbContextFactory.Create())
            {
                var category = new CategoryBuilder().WithId(1).WithName("Flowers").Build();
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
            using (var context = TestDbContextFactory.Create())
            {
                var category = new CategoryBuilder().WithId(1).WithName("Flowers").Build();
                context.Categories.Add(category);
                var toy = new ToyBuilder()
                    .WithId(1)
                    .WithName("Old Name")
                    .WithCategoryId(1)
                    .WithPrice(10.00m)
                    .WithIsAvailable(true)
                    .WithCreatedByUserId("user123")
                    .Build();
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
                var updated = await context.Toys.FindAsync(1)
                    ?? throw new InvalidOperationException("Expected toy to exist after edit.");
                Assert.Equal("Updated Rose", updated.Name);
            }
        }

        [Fact]
        public async Task EditAsync_WithUnauthorizedUser_ReturnsFalse()
        {
            // Arrange
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                var toy = new ToyBuilder()
                    .WithId(1)
                    .WithName("Old Name")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("otherUser")
                    .Build();
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
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                var toy = new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("user123")
                    .Build();
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
            using (var context = TestDbContextFactory.Create())
            {
                var service = new ToyService(context);

                // Act
                var result = await service.DeleteAsync(999, "user123");

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task EditAsync_AllowsAdmin()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(
                new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("owner-1")
                    .Build());
            context.SaveChanges();

            var service = new ToyService(context);
            var model = new CrochetToysShop.Web.ViewModels.Toys.ToyFormViewModel
            {
                Name = "Edited By Admin",
                Description = "Updated by admin with valid description text.",
                CategoryId = 1,
                Price = 22.50m,
                SizeCm = 21,
                Difficulty = "Medium",
                IsAvailable = true
            };

            // Act
            var result = await service.EditAsync(1, model, userId: "not-owner", isAdmin: true);

            // Assert
            Assert.True(result);
            var updatedToy = await context.Toys.FindAsync(1);
            Assert.NotNull(updatedToy);
            Assert.Equal("Edited By Admin", updatedToy.Name);
        }

        [Fact]
        public async Task EditAsync_AllowsOwner()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(
                new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("owner-1")
                    .Build());
            context.SaveChanges();

            var service = new ToyService(context);
            var model = new CrochetToysShop.Web.ViewModels.Toys.ToyFormViewModel
            {
                Name = "Edited By Owner",
                Description = "Updated by owner with valid description text.",
                CategoryId = 1,
                Price = 25.00m,
                SizeCm = 23,
                Difficulty = "Hard",
                IsAvailable = false
            };

            // Act
            var result = await service.EditAsync(1, model, userId: "owner-1", isAdmin: false);

            // Assert
            Assert.True(result);
            var updatedToy = await context.Toys.FindAsync(1);
            Assert.NotNull(updatedToy);
            Assert.Equal("Edited By Owner", updatedToy.Name);
        }

        [Fact]
        public async Task EditAsync_DeniesNonOwner()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(
                new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("owner-1")
                    .Build());
            context.SaveChanges();

            var service = new ToyService(context);
            var model = new CrochetToysShop.Web.ViewModels.Toys.ToyFormViewModel
            {
                Name = "Should Not Update",
                Description = "Non-owner tries to update with valid description.",
                CategoryId = 1,
                Price = 30.00m,
                SizeCm = 25,
                Difficulty = "Easy",
                IsAvailable = true
            };

            // Act
            var result = await service.EditAsync(1, model, userId: "other-user", isAdmin: false);

            // Assert
            Assert.False(result);
            var originalToy = await context.Toys.FindAsync(1);
            Assert.NotNull(originalToy);
            Assert.Equal("Rose", originalToy.Name);
        }

        [Fact]
        public async Task EditAsync_WithMissingToy_ReturnsFalse()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var service = new ToyService(context);
            var model = new CrochetToysShop.Web.ViewModels.Toys.ToyFormViewModel
            {
                Name = "Missing",
                Description = "Trying to edit missing toy with valid description.",
                CategoryId = 1,
                Price = 10.00m,
                SizeCm = 10,
                Difficulty = "Easy",
                IsAvailable = true
            };

            // Act
            var result = await service.EditAsync(999, model, userId: "owner-1", isAdmin: false);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_AllowsAdmin()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(
                new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("owner-1")
                    .Build());
            context.SaveChanges();

            var service = new ToyService(context);

            // Act
            var result = await service.DeleteAsync(1, userId: "not-owner", isAdmin: true);

            // Assert
            Assert.True(result);
            var deletedToy = await context.Toys.FindAsync(1);
            Assert.Null(deletedToy);
        }

        [Fact]
        public async Task DeleteAsync_AllowsOwner()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(
                new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("owner-1")
                    .Build());
            context.SaveChanges();

            var service = new ToyService(context);

            // Act
            var result = await service.DeleteAsync(1, userId: "owner-1", isAdmin: false);

            // Assert
            Assert.True(result);
            var deletedToy = await context.Toys.FindAsync(1);
            Assert.Null(deletedToy);
        }

        [Fact]
        public async Task DeleteAsync_DeniesNonOwner()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(
                new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("owner-1")
                    .Build());
            context.SaveChanges();

            var service = new ToyService(context);

            // Act
            var result = await service.DeleteAsync(1, userId: "other-user", isAdmin: false);

            // Assert
            Assert.False(result);
            var existingToy = await context.Toys.FindAsync(1);
            Assert.NotNull(existingToy);
        }

        [Fact]
        public async Task SearchAsync_WithSearchTerm_ReturnsMatchingNameOrDescription()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose Doll").WithDescription("Soft pink petals").WithCategoryId(1).Build());
            context.Toys.Add(new ToyBuilder().WithId(2).WithName("Tulip").WithDescription("Rose style bouquet").WithCategoryId(1).Build());
            context.Toys.Add(new ToyBuilder().WithId(3).WithName("Fox").WithDescription("Forest friend").WithCategoryId(1).Build());
            context.SaveChanges();

            var service = new ToyService(context);

            // Act
            var result = await service.SearchAsync("rOsE");

            // Assert
            Assert.Equal(2, result.Toys.Count());
            Assert.Contains(result.Toys, t => t.Name == "Rose Doll");
            Assert.Contains(result.Toys, t => t.Name == "Tulip");
        }

        [Fact]
        public async Task GetByCategoryNameAsync_WithTrimmedCategoryName_ReturnsOnlyMatchingToys()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Categories.Add(new CategoryBuilder().WithId(2).WithName("Animals").Build());
            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).Build());
            context.Toys.Add(new ToyBuilder().WithId(2).WithName("Tulip").WithCategoryId(1).Build());
            context.Toys.Add(new ToyBuilder().WithId(3).WithName("Fox").WithCategoryId(2).Build());
            context.SaveChanges();

            var service = new ToyService(context);

            // Act
            var result = await service.GetByCategoryNameAsync("  fLoWeRs  ");

            // Assert
            Assert.Equal(2, result.Toys.Count());
            Assert.All(result.Toys, t => Assert.Equal("Flowers", t.CategoryName));
        }

        [Fact]
        public async Task GetCreateModelAsync_ReturnsCategories()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Categories.Add(new CategoryBuilder().WithId(2).WithName("Animals").Build());
            context.SaveChanges();

            var service = new ToyService(context);

            // Act
            var result = await service.GetCreateModelAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Categories.Count());
        }

        [Fact]
        public async Task GetEditModelAsync_WithOwner_ReturnsModel()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(
                new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("owner-1")
                    .Build());
            context.SaveChanges();

            var service = new ToyService(context);

            // Act
            var result = await service.GetEditModelAsync(1, "owner-1", isAdmin: false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rose", result.Name);
            Assert.Equal(1, result.CategoryId);
        }

        [Fact]
        public async Task GetEditModelAsync_WithUnauthorizedUser_ReturnsNull()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(
                new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("owner-1")
                    .Build());
            context.SaveChanges();

            var service = new ToyService(context);

            // Act
            var result = await service.GetEditModelAsync(1, "other-user", isAdmin: false);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetDeleteModelAsync_WithAdmin_ReturnsModel()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(
                new ToyBuilder()
                    .WithId(1)
                    .WithName("Rose")
                    .WithCategoryId(1)
                    .WithCreatedByUserId("owner-1")
                    .Build());
            context.SaveChanges();

            var service = new ToyService(context);

            // Act
            var result = await service.GetDeleteModelAsync(1, "any-user", isAdmin: true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Rose", result.Name);
        }

        [Fact]
        public async Task GetDeleteModelAsync_WithMissingToy_ReturnsNull()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var service = new ToyService(context);

            // Act
            var result = await service.GetDeleteModelAsync(999, "owner-1", isAdmin: false);

            // Assert
            Assert.Null(result);
        }
    }
}
