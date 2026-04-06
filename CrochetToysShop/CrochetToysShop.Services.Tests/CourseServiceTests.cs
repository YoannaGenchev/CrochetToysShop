namespace CrochetToysShop.Services.Tests
{
    using CrochetToysShop.Data.Models;
    using CrochetToysShop.Services.Core;
    using CrochetToysShop.Services.Tests.Infrastructure;
    using CrochetToysShop.Services.Tests.Infrastructure.Builders;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class CourseServiceTests
    {
        [Fact]
        public async Task GetAllAsync_WithNoFilter_ReturnsActiveCourses()
        {
            // Arrange
            using (var context = TestDbContextFactory.Create())
            {
                context.Courses.Add(new CourseBuilder().WithId(1).WithName("Beginner Crochet").WithDifficulty("Beginner").WithIsActive(true).WithMaxStudents(20).Build());
                context.Courses.Add(new CourseBuilder().WithId(2).WithName("Advanced Crochet").WithDifficulty("Advanced").WithIsActive(true).WithMaxStudents(15).Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                context.Courses.Add(new CourseBuilder().WithId(1).WithName("Beginner Crochet").WithDifficulty("Beginner").WithIsActive(true).WithMaxStudents(20).Build());
                context.Courses.Add(new CourseBuilder().WithId(2).WithName("Advanced Crochet").WithDifficulty("Advanced").WithIsActive(true).WithMaxStudents(15).Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                for (int i = 1; i <= 25; i++)
                {
                    context.Courses.Add(
                        new CourseBuilder()
                            .WithId(i)
                            .WithName($"Course{i}")
                            .WithDifficulty("Beginner")
                            .WithIsActive(true)
                            .WithMaxStudents(20)
                            .WithPrice(50.00m)
                            .Build());
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
            using (var context = TestDbContextFactory.Create())
            {
                var course = new CourseBuilder()
                    .WithId(1)
                    .WithName("Beginner Crochet")
                    .WithDescription("Learn the basics")
                    .WithDifficulty("Beginner")
                    .WithIsActive(true)
                    .WithMaxStudents(20)
                    .WithPrice(50.00m)
                    .WithDurationHours(10)
                    .Build();
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
            using (var context = TestDbContextFactory.Create())
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
            using (var context = TestDbContextFactory.Create())
            {
                var course = new CourseBuilder().WithId(1).WithMaxStudents(2).Build();
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
            using (var context = TestDbContextFactory.Create())
            {
                var course = new CourseBuilder().WithId(1).WithMaxStudents(1).Build();
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
            using (var context = TestDbContextFactory.Create())
            {
                var course = new CourseBuilder().WithId(1).WithMaxStudents(20).Build();
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
            using (var context = TestDbContextFactory.Create())
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
            using (var context = TestDbContextFactory.Create())
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
            using (var context = TestDbContextFactory.Create())
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

        [Fact]
        public async Task EnrollAsync_WithInactiveCourse_ReturnsFalse()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Courses.Add(new CourseBuilder().WithId(1).WithName("Inactive Course").WithIsActive(false).WithMaxStudents(20).Build());
            context.SaveChanges();

            var service = new CourseService(context);

            // Act
            var result = await service.EnrollAsync(1, "user1");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EnrollAsync_WhenCapacityIsReachedAfterLastSpot_ReturnsFalseForNextUser()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Courses.Add(new CourseBuilder().WithId(1).WithMaxStudents(2).Build());
            context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "user1" });
            context.SaveChanges();

            var service = new CourseService(context);

            // Act
            var firstResult = await service.EnrollAsync(1, "user2");
            var secondResult = await service.EnrollAsync(1, "user3");

            // Assert
            Assert.True(firstResult);
            Assert.False(secondResult);
        }

        [Fact]
        public async Task GetEnrolledCoursesAsync_WithEnrollments_ReturnsOnlyCurrentUserCoursesOrderedByDate()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Courses.Add(new CourseBuilder().WithId(1).WithName("Beginner Crochet").WithDifficulty("Beginner").WithDurationHours(8).WithPrice(40m).Build());
            context.Courses.Add(new CourseBuilder().WithId(2).WithName("Advanced Crochet").WithDifficulty("Advanced").WithDurationHours(12).WithPrice(60m).Build());
            context.Courses.Add(new CourseBuilder().WithId(3).WithName("Blanket Course").WithDifficulty("Intermediate").WithDurationHours(10).WithPrice(50m).Build());

            context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "user1", EnrolledAt = new DateTime(2026, 1, 10), IsCompleted = false });
            context.Enrollments.Add(new Enrollment { CourseId = 2, UserId = "user1", EnrolledAt = new DateTime(2026, 2, 1), IsCompleted = true });
            context.Enrollments.Add(new Enrollment { CourseId = 3, UserId = "user2", EnrolledAt = new DateTime(2026, 3, 1), IsCompleted = false });
            context.SaveChanges();

            var service = new CourseService(context);

            // Act
            var result = (await service.GetEnrolledCoursesAsync("user1")).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Advanced Crochet", result[0].Name);
            Assert.Equal("Beginner Crochet", result[1].Name);
            Assert.True(result[0].IsCompleted);
            Assert.False(result[1].IsCompleted);
        }

        [Fact]
        public async Task GetEnrolledCoursesAsync_WithNoEnrollments_ReturnsEmptyCollection()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var service = new CourseService(context);

            // Act
            var result = await service.GetEnrolledCoursesAsync("user1");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllEnrollmentsForAdminAsync_ReturnsMappedRowsOrderedByDate()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Courses.Add(new CourseBuilder().WithId(1).WithName("Beginner Crochet").Build());
            context.Courses.Add(new CourseBuilder().WithId(2).WithName("Advanced Crochet").Build());

            context.Users.Add(new IdentityUser { Id = "user1", Email = "student1@example.com", UserName = "student1@example.com" });
            context.Users.Add(new IdentityUser { Id = "user2", Email = "student2@example.com", UserName = "student2@example.com" });

            context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "user1", EnrolledAt = new DateTime(2026, 1, 5), IsCompleted = false });
            context.Enrollments.Add(new Enrollment { CourseId = 2, UserId = "user2", EnrolledAt = new DateTime(2026, 2, 8), IsCompleted = true });
            context.SaveChanges();

            var service = new CourseService(context);

            // Act
            var result = (await service.GetAllEnrollmentsForAdminAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Advanced Crochet", result[0].CourseName);
            Assert.Equal("student2@example.com", result[0].EnrolledUser);
            Assert.True(result[0].IsCompleted);
            Assert.Equal("Beginner Crochet", result[1].CourseName);
            Assert.Equal("student1@example.com", result[1].EnrolledUser);
        }

        [Fact]
        public async Task GetAllEnrollmentsForAdminAsync_WhenUserIsMissing_FallsBackToUserId()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Courses.Add(new CourseBuilder().WithId(1).WithName("Beginner Crochet").Build());
            context.Enrollments.Add(new Enrollment { CourseId = 1, UserId = "missing-user", EnrolledAt = new DateTime(2026, 3, 1) });
            context.SaveChanges();

            var service = new CourseService(context);

            // Act
            var result = (await service.GetAllEnrollmentsForAdminAsync()).Single();

            // Assert
            Assert.Equal("missing-user", result.EnrolledUser);
        }
    }
}
