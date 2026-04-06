namespace CrochetToysShop.Data.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string UserId { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public virtual Course Course { get; set; } = null!;
    }
}
