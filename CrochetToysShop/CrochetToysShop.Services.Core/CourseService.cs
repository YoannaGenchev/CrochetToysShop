using CrochetToysShop.Data;
using CrochetToysShop.Services.Core.Interfaces;
using CrochetToysShop.Web.ViewModels.Courses;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Services.Core
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext db;

        public CourseService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<CourseIndexViewModel> GetAllAsync(string? difficultyFilter = null, int page = 1, int pageSize = 10)
        {
            var query = db.Courses
                .AsNoTracking()
                .Where(c => c.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(difficultyFilter))
            {
                query = query.Where(c => c.Difficulty == difficultyFilter);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var courses = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseListItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    DurationHours = c.DurationHours,
                    Difficulty = c.Difficulty,
                    ImageUrl = c.ImageUrl,
                    EnrolledCount = c.Enrollments.Count,
                    MaxStudents = c.MaxStudents
                })
                .ToListAsync();

            return new CourseIndexViewModel
            {
                Courses = courses,
                DifficultyFilter = difficultyFilter,
                Pagination = new Web.ViewModels.Common.PaginationViewModel
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<CourseDetailsViewModel?> GetDetailsAsync(int id, string? userId = null)
        {
            var course = await db.Courses
                .AsNoTracking()
                .Where(c => c.Id == id && c.IsActive)
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return null;
            }

            var enrolledCount = await db.Enrollments
                .AsNoTracking()
                .CountAsync(e => e.CourseId == id);

            var isUserEnrolled = false;
            if (!string.IsNullOrEmpty(userId))
            {
                isUserEnrolled = await db.Enrollments
                    .AsNoTracking()
                    .AnyAsync(e => e.CourseId == id && e.UserId == userId);
            }

            return new CourseDetailsViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Price = course.Price,
                DurationHours = course.DurationHours,
                Difficulty = course.Difficulty,
                ImageUrl = course.ImageUrl,
                MaxStudents = course.MaxStudents,
                EnrolledCount = enrolledCount,
                IsUserEnrolled = isUserEnrolled
            };
        }

        public async Task<bool> EnrollAsync(int courseId, string userId)
        {
            // Check if course exists and has space
            var course = await db.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return false;
            }

            var enrolledCount = await db.Enrollments
                .CountAsync(e => e.CourseId == courseId);

            if (enrolledCount >= course.MaxStudents)
            {
                return false;
            }

            // Check if already enrolled
            var existingEnrollment = await db.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

            if (existingEnrollment != null)
            {
                return false;
            }

            var enrollment = new CrochetToysShop.Data.Models.Enrollment
            {
                CourseId = courseId,
                UserId = userId
            };

            await db.Enrollments.AddAsync(enrollment);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsEnrolledAsync(int courseId, string userId)
        {
            return await db.Enrollments
                .AsNoTracking()
                .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        }

        public async Task<int> GetEnrolledCountAsync(int courseId)
        {
            return await db.Enrollments
                .AsNoTracking()
                .CountAsync(e => e.CourseId == courseId);
        }
    }
}
