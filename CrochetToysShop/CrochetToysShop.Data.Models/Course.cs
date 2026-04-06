namespace CrochetToysShop.Data.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int DurationHours { get; set; }

        public string Difficulty { get; set; } = "Beginner";

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int MaxStudents { get; set; } = 20;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
