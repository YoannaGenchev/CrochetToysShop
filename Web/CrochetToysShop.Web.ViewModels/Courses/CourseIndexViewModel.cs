namespace CrochetToysShop.Web.ViewModels.Courses
{
    public class CourseIndexViewModel
    {
        public IEnumerable<CourseListItemViewModel> Courses { get; set; } = new List<CourseListItemViewModel>();

        public string? DifficultyFilter { get; set; }

        public IEnumerable<string> AvailableDifficulties { get; set; } = new[] { "Beginner", "Intermediate", "Advanced" };
    }
}
