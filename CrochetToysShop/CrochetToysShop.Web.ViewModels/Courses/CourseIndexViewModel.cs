namespace CrochetToysShop.Web.ViewModels.Courses
{
    using CrochetToysShop.Web.ViewModels.Common;

    public class CourseIndexViewModel
    {
        public IEnumerable<CourseListItemViewModel> Courses { get; set; } = new List<CourseListItemViewModel>();

        public string? DifficultyFilter { get; set; }

        public IEnumerable<string> AvailableDifficulties { get; set; } = new[] { "Beginner", "Intermediate", "Advanced" };

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
