namespace CrochetToysShop.Services.Tests
{
    using CrochetToysShop.Common.Constants;
    using CrochetToysShop.Data;
    using CrochetToysShop.Data.Models;
    using CrochetToysShop.Services.Core;
    using CrochetToysShop.Web.ViewModels.Orders;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class OrderServiceTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllForAdminAsync_ReturnsAllOrders()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var toy = new Toy { Id = 1, Name = "Rose" };
                context.Toys.Add(toy);
                context.Orders.Add(new Order { Id = 1, CustomerName = "John", Status = "New", CreatedOn = DateTime.Now, ToyId = 1 });
                context.Orders.Add(new Order { Id = 2, CustomerName = "Jane", Status = "Completed", CreatedOn = DateTime.Now, ToyId = 1 });
                context.SaveChanges();

                var service = new OrderService(context);

                // Act
                var result = await service.GetAllForAdminAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Orders.Count());
            }
        }

        [Fact]
        public async Task GetAllForAdminAsync_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var toy = new Toy { Id = 1, Name = "Rose" };
                context.Toys.Add(toy);
                for (int i = 1; i <= 15; i++)
                {
                    context.Orders.Add(new Order
                    {
                        Id = i,
                        CustomerName = $"Customer{i}",
                        Status = "New",
                        CreatedOn = DateTime.Now,
                        ToyId = 1,
                        PhoneNumber = "123456",
                        Address = "Address"
                    });
                }
                context.SaveChanges();

                var service = new OrderService(context);

                // Act
                var result = await service.GetAllForAdminAsync(page: 1, pageSize: 10);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(10, result.Orders.Count());
                Assert.Equal(2, result.Pagination.TotalPages);
                Assert.Equal(1, result.Pagination.CurrentPage);
                Assert.Equal(15, result.Pagination.TotalCount);
            }
        }

        [Fact]
        public async Task GetOrderModelAsync_WithAvailableToy_ReturnsOrderModel()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                context.Toys.Add(new Toy { Id = 1, Name = "Rose", IsAvailable = true });
                context.SaveChanges();

                var service = new OrderService(context);

                // Act
                var result = await service.GetOrderModelAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.ToyId);
                Assert.Equal("Rose", result.ToyName);
            }
        }

        [Fact]
        public async Task GetOrderModelAsync_WithUnavailableToy_ReturnsNull()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                context.Toys.Add(new Toy { Id = 1, Name = "Rose", IsAvailable = false });
                context.SaveChanges();

                var service = new OrderService(context);

                // Act
                var result = await service.GetOrderModelAsync(1);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task CreateOrderAsync_WithAvailableToy_CreatesOrder()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                context.Toys.Add(new Toy { Id = 1, Name = "Rose", IsAvailable = true });
                context.SaveChanges();

                var service = new OrderService(context);
                var model = new OrderCreateViewModel
                {
                    ToyId = 1,
                    CustomerName = "John",
                    PhoneNumber = "123456",
                    Address = "123 Main St"
                };

                // Act
                var (ok, error, toyId) = await service.CreateOrderAsync(model);

                // Assert
                Assert.True(ok);
                Assert.Null(error);
                Assert.Equal(1, toyId);
                var createdOrder = await context.Orders.FirstOrDefaultAsync();
                Assert.NotNull(createdOrder);
            }
        }

        [Fact]
        public async Task CreateOrderAsync_WithUnavailableToy_ReturnsFalse()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                context.Toys.Add(new Toy { Id = 1, Name = "Rose", IsAvailable = false });
                context.SaveChanges();

                var service = new OrderService(context);
                var model = new OrderCreateViewModel
                {
                    ToyId = 1,
                    CustomerName = "John",
                    PhoneNumber = "123456",
                    Address = "123 Main St"
                };

                // Act
                var (ok, error, toyId) = await service.CreateOrderAsync(model);

                // Assert
                Assert.False(ok);
                Assert.NotNull(error);
                Assert.Equal(ApplicationConstants.ErrorMessages.ToyNotAvailable, error);
            }
        }

        [Fact]
        public async Task CreateOrderAsync_WithNonexistentToy_ReturnsFalse()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var service = new OrderService(context);
                var model = new OrderCreateViewModel
                {
                    ToyId = 999,
                    CustomerName = "John",
                    PhoneNumber = "123456",
                    Address = "123 Main St"
                };

                // Act
                var (ok, error, toyId) = await service.CreateOrderAsync(model);

                // Assert
                Assert.False(ok);
                Assert.NotNull(error);
                Assert.Equal(ApplicationConstants.ErrorMessages.ToyNotFound, error);
            }
        }

        [Fact]
        public async Task MarkCompletedAsync_WithValidId_MarksOrderAsCompleted()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var toy = new Toy { Id = 1, Name = "Rose" };
                context.Toys.Add(toy);
                context.Orders.Add(new Order { Id = 1, Status = "New", CreatedOn = DateTime.Now, ToyId = 1, CustomerName = "John", PhoneNumber = "123", Address = "123" });
                context.SaveChanges();

                var service = new OrderService(context);

                // Act
                var result = await service.MarkCompletedAsync(1);

                // Assert
                Assert.True(result);
                var updated = await context.Orders.FindAsync(1);
                Assert.Equal("Completed", updated.Status);
            }
        }

        [Fact]
        public async Task MarkCompletedAsync_WithNonexistentId_ReturnsFalse()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var service = new OrderService(context);

                // Act
                var result = await service.MarkCompletedAsync(999);

                // Assert
                Assert.False(result);
            }
        }
    }
}
