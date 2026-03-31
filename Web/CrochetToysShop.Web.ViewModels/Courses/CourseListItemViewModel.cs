namespace CrochetToysShop.Web.ViewModels.Courses
{
    public class CourseListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationHours { get; set; }

        public string Difficulty { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public int EnrolledCount { get; set; }

        public int MaxStudents { get; set; }
    }
}
