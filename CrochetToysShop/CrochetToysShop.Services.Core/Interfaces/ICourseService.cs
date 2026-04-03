using CrochetToysShop.Web.ViewModels.Courses;

namespace CrochetToysShop.Services.Core.Interfaces
{
    public interface ICourseService
    {
        Task<CourseIndexViewModel> GetAllAsync(string? difficultyFilter = null, int page = 1, int pageSize = 10);

        Task<CourseDetailsViewModel?> GetDetailsAsync(int id, string? userId = null);

        Task<bool> EnrollAsync(int courseId, string userId);

        Task<bool> IsEnrolledAsync(int courseId, string userId);

        Task<int> GetEnrolledCountAsync(int courseId);
    }
}
