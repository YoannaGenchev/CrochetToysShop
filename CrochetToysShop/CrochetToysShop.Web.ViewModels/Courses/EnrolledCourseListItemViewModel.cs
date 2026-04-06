namespace CrochetToysShop.Web.ViewModels.Courses
{
    public class EnrolledCourseListItemViewModel
    {
        public int CourseId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Difficulty { get; set; } = string.Empty;

        public int DurationHours { get; set; }

        public decimal Price { get; set; }

        public DateTime EnrolledAt { get; set; }

        public bool IsCompleted { get; set; }
    }
}
