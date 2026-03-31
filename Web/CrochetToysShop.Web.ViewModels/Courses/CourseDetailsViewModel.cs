namespace CrochetToysShop.Web.ViewModels.Courses
{
    public class CourseDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationHours { get; set; }

        public string Difficulty { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public int MaxStudents { get; set; }

        public int EnrolledCount { get; set; }

        public bool CanEnroll => EnrolledCount < MaxStudents;

        public bool IsUserEnrolled { get; set; }
    }
}
