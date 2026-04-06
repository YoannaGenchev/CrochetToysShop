namespace CrochetToysShop.Web.ViewModels.Courses
{
    public class AdminCourseEnrollmentListItemViewModel
    {
        public string CourseName { get; set; } = string.Empty;

        public string EnrolledUser { get; set; } = string.Empty;

        public DateTime EnrolledAt { get; set; }

        public bool IsCompleted { get; set; }
    }
}
