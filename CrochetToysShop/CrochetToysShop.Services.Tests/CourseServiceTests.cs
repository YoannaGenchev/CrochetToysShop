namespace CrochetToysShop.Services.Tests
{
    using CrochetToysShop.Data;
    using CrochetToysShop.Data.Models;
    using CrochetToysShop.Services.Core;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class CourseServiceTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_WithNoFilter_ReturnsActiveCourses()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                context.Courses.Add(new Course { Id = 1, Name = "Beginner Crochet", Difficulty = "Beginner", IsActive = true, MaxStudents = 20 });
                context.Courses.Add(new Course { Id = 2, Name = "Advanced Crochet", Difficulty = "Advanced", IsActive = true, MaxStudents = 15 });
                context.SaveChanges();

                var service = new CourseService(context);

                // Act
                var result = await service.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Courses.Count());
            }
        }

        [Fact]
        public async Task GetAllAsync_WithDifficultyFilter_ReturnsFilturedCourses()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                context.Courses.Add(new Course { Id = 1, Name = "Beginner Crochet", Difficulty = "Beginner", IsActive = true, MaxStudents = 20 });
                context.Courses.Add(new Course { Id = 2, Name = "Advanced Crochet", Difficulty = "Advanced", IsActive = true, MaxStudents = 15 });
                context.SaveChanges();

                var service = new CourseService(context);

                // Act
                var result = await service.GetAllAsync("Beginner");

                // Assert
                Assert.NotNull(result);
                Assert.Single(result.Courses);
                Assert.Equal("Beginner Crochet", result.Courses.First().Name);
            }
        }

        [Fact]
        public async Task GetAllAsync_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                for (int i = 1; i <= 25; i++)
                {
                    context.Courses.Add(new Course
                    {
                        Id = i,
                        Name = $"Course{i}",
                        Difficulty = "Beginner",
                        IsActive = true,
                        MaxStudents = 20,
                        Price = 50.00m
                    });
                }
                context.SaveChanges();

                var service = new CourseService(context);

                // Act
                var result = await service.GetAllAsync(page: 2, pageSize: 10);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(10, result.Courses.Count());
                Assert.Equal(3, result.Pagination.TotalPages);
                Assert.Equal(2, result.Pagination.CurrentPage);
                Assert.Equal(25, result.Pagination.TotalCount);
            }
        }

        [Fact]
        public async Task GetDetailsAsync_WithValidId_ReturnsCorrectCourse()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var course = new Course
                {
                    Id = 1,
                    Name = "Beginner Crochet",
                    Description = "Learn the basics",
                    Difficulty = "Beginner",
                    IsActive = true,
                    MaxStudents = 20,
                    Price = 50.00m,
                    DurationHours = 10
                };
                context.Courses.Add(course);
                context.SaveChanges();

                var service = new CourseService(context);

                // Act
                var result = await service.GetDetailsAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Beginner Crochet", result.Name);
                Assert.Equal("Learn the basics", result.Description);
            }
        }

        [Fact]
        public async Task GetDetailsAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var service = new CourseService(context);

                // Act
                var result = await service.GetDetailsAsync(999);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task EnrollAsync_WithAvailableSpace_EnrollsUserSuccessfully()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var course = new Course { Id = 1, MaxStudents = 2 };
                context.Courses.Add(course);
                context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "user1" });
                context.SaveChanges();

                var service = new CourseService(context);

                // Act
                var result = await service.EnrollAsync(1, "user2");

                // Assert
                Assert.True(result);
                var enrollment = await context.Enrollments.FirstOrDefaultAsync(e => e.UserId == "user2");
                Assert.NotNull(enrollment);
            }
        }

        [Fact]
        public async Task EnrollAsync_WithNoAvailableSpace_ReturnsFalse()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var course = new Course { Id = 1, MaxStudents = 1 };
                context.Courses.Add(course);
                context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "user1" });
                context.SaveChanges();

                var service = new CourseService(context);

                // Act
                var result = await service.EnrollAsync(1, "user2");

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task EnrollAsync_WhenAlreadyEnrolled_ReturnsFalse()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var course = new Course { Id = 1, MaxStudents = 20 };
                context.Courses.Add(course);
                context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "user1" });
                context.SaveChanges();

                var service = new CourseService(context);

                // Act
                var result = await service.EnrollAsync(1, "user1");

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task IsEnrolledAsync_WhenUserIsEnrolled_ReturnsTrue()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "user1" });
                context.SaveChanges();

                var service = new CourseService(context);

                // Act
                var result = await service.IsEnrolledAsync(1, "user1");

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task IsEnrolledAsync_WhenUserIsNotEnrolled_ReturnsFalse()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                var service = new CourseService(context);

                // Act
                var result = await service.IsEnrolledAsync(1, "user1");

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task GetEnrolledCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            using (var context = CreateInMemoryDbContext())
            {
                context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "user1" });
                context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "user2" });
                context.Enrollments.Add(new Enrollment { CourseId = 2, UserId = "user3" });
                context.SaveChanges();

                var service = new CourseService(context);

                // Act
                var result = await service.GetEnrolledCountAsync(1);

                // Assert
                Assert.Equal(2, result);
            }
        }
    }
}
