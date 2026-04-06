namespace CrochetToysShop.Services.Tests
{
    using CrochetToysShop.Common;
    using CrochetToysShop.Common.Constants;
    using CrochetToysShop.Services.Core;
    using CrochetToysShop.Services.Core.Interfaces;
    using CrochetToysShop.Services.Tests.Infrastructure;
    using CrochetToysShop.Services.Tests.Infrastructure.Builders;
    using CrochetToysShop.Web.ViewModels.Orders;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class OrderServiceTests
    {
        [Fact]
        public async Task GetAllForAdminAsync_ReturnsAllOrders()
        {
            // Arrange
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                var toy = new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).Build();
                context.Toys.Add(toy);
                context.Orders.Add(new OrderBuilder().WithId(1).WithCustomerName("John").WithStatus("New").WithCreatedOn(DateTime.UtcNow).WithToyId(1).Build());
                context.Orders.Add(new OrderBuilder().WithId(2).WithCustomerName("Jane").WithStatus("Completed").WithCreatedOn(DateTime.UtcNow).WithToyId(1).Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                var toy = new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).Build();
                context.Toys.Add(toy);
                for (int i = 1; i <= 15; i++)
                {
                    context.Orders.Add(
                        new OrderBuilder()
                            .WithId(i)
                            .WithCustomerName($"Customer{i}")
                            .WithStatus("New")
                            .WithCreatedOn(DateTime.UtcNow)
                            .WithToyId(1)
                            .WithPhoneNumber("123456")
                            .WithAddress("Address")
                            .Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithIsAvailable(true).Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithIsAvailable(false).Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithIsAvailable(true).Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithIsAvailable(false).Build());
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
            using (var context = TestDbContextFactory.Create())
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
            using (var context = TestDbContextFactory.Create())
            {
                context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
                var toy = new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).Build();
                context.Toys.Add(toy);
                context.Orders.Add(
                    new OrderBuilder()
                        .WithId(1)
                        .WithStatus("New")
                        .WithCreatedOn(DateTime.UtcNow)
                        .WithToyId(1)
                        .WithCustomerName("John")
                        .WithPhoneNumber("123")
                        .WithAddress("123")
                        .Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                var service = new OrderService(context);

                // Act
                var result = await service.MarkCompletedAsync(999);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task CreateOrderAsync_WithAvailableToy_CreatesOrderSuccessfully()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithIsAvailable(true).Build());
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
            Assert.Single(context.Orders);
            var createdOrder = await context.Orders.FirstAsync();
            Assert.Equal(OrderStatus.New, createdOrder.Status);
            Assert.Equal("John", createdOrder.CustomerName);
        }

        [Fact]
        public async Task CreateOrderAsync_WithAvailableToy_MarksToyAsUnavailable()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithIsAvailable(true).Build());
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
            var (ok, _, _) = await service.CreateOrderAsync(model);

            // Assert
            Assert.True(ok);
            var toy = await context.Toys.FindAsync(1);
            Assert.NotNull(toy);
            Assert.False(toy.IsAvailable);
        }

        [Fact]
        public async Task CreateOrderAsync_WithMissingToy_ReturnsExpectedFailure()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

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
            Assert.Equal(ApplicationConstants.ErrorMessages.ToyNotFound, error);
            Assert.Equal(999, toyId);
            Assert.Empty(context.Orders);
        }

        [Fact]
        public async Task CreateOrderAsync_WithUnavailableToy_ReturnsExpectedFailure()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithIsAvailable(false).Build());
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
            Assert.Equal(ApplicationConstants.ErrorMessages.ToyNotAvailable, error);
            Assert.Equal(1, toyId);
            Assert.Empty(context.Orders);
        }

        [Fact]
        public async Task MarkCompletedAsync_WithExistingOrder_UpdatesStatusCorrectly()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).Build());
            context.Orders.Add(new OrderBuilder().WithId(1).WithToyId(1).WithStatus(OrderStatus.New).Build());
            context.SaveChanges();

            var service = new OrderService(context);

            // Act
            var ok = await service.MarkCompletedAsync(1);

            // Assert
            Assert.True(ok);
            var updatedOrder = await context.Orders.FindAsync(1);
            Assert.NotNull(updatedOrder);
            Assert.Equal(OrderStatus.Completed, updatedOrder.Status);
        }

        [Fact]
        public async Task MarkCompletedAsync_WithMissingOrder_ReturnsExpectedFailure()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var service = new OrderService(context);

            // Act
            var ok = await service.MarkCompletedAsync(404);

            // Assert
            Assert.False(ok);
        }

        [Fact]
        public async Task CreateOrderAsync_WithAvailableToy_CallsNotificationService()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithIsAvailable(true).Build());
            context.SaveChanges();

            var notificationServiceMock = new Mock<INotificationService>();
            var service = new OrderService(context, notificationServiceMock.Object);
            var model = new OrderCreateViewModel
            {
                ToyId = 1,
                CustomerName = "John",
                PhoneNumber = "123456",
                Address = "123 Main St"
            };

            // Act
            var (ok, _, _) = await service.CreateOrderAsync(model);

            // Assert
            Assert.True(ok);
            notificationServiceMock.Verify(
                n => n.NotifyOrderCreatedAsync(It.IsAny<int>(), 1, "John"),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_WithMissingToy_DoesNotCallNotificationService()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            var notificationServiceMock = new Mock<INotificationService>();
            var service = new OrderService(context, notificationServiceMock.Object);
            var model = new OrderCreateViewModel
            {
                ToyId = 999,
                CustomerName = "John",
                PhoneNumber = "123456",
                Address = "123 Main St"
            };

            // Act
            var (ok, _, _) = await service.CreateOrderAsync(model);

            // Assert
            Assert.False(ok);
            notificationServiceMock.Verify(
                n => n.NotifyOrderCreatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_WithUnavailableToy_DoesNotCallNotificationService()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Categories.Add(new CategoryBuilder().WithId(1).WithName("Flowers").Build());
            context.Toys.Add(new ToyBuilder().WithId(1).WithName("Rose").WithCategoryId(1).WithIsAvailable(false).Build());
            context.SaveChanges();

            var notificationServiceMock = new Mock<INotificationService>();
            var service = new OrderService(context, notificationServiceMock.Object);
            var model = new OrderCreateViewModel
            {
                ToyId = 1,
                CustomerName = "John",
                PhoneNumber = "123456",
                Address = "123 Main St"
            };

            // Act
            var (ok, _, _) = await service.CreateOrderAsync(model);

            // Assert
            Assert.False(ok);
            notificationServiceMock.Verify(
                n => n.NotifyOrderCreatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}
