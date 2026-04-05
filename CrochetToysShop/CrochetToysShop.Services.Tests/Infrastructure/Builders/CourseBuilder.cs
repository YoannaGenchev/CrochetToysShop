using CrochetToysShop.Data.Models;

namespace CrochetToysShop.Services.Tests.Infrastructure.Builders
{
    internal class CourseBuilder
    {
        private readonly Course course = new()
        {
            Name = "Test Course",
            Description = "Valid course description.",
            Price = 50.00m,
            DurationHours = 10,
            Difficulty = "Beginner",
            IsActive = true,
            MaxStudents = 20
        };

        public CourseBuilder WithId(int id)
        {
            course.Id = id;
            return this;
        }

        public CourseBuilder WithName(string name)
        {
            course.Name = name;
            return this;
        }

        public CourseBuilder WithDescription(string description)
        {
            course.Description = description;
            return this;
        }

        public CourseBuilder WithDifficulty(string difficulty)
        {
            course.Difficulty = difficulty;
            return this;
        }

        public CourseBuilder WithIsActive(bool isActive)
        {
            course.IsActive = isActive;
            return this;
        }

        public CourseBuilder WithMaxStudents(int maxStudents)
        {
            course.MaxStudents = maxStudents;
            return this;
        }

        public CourseBuilder WithPrice(decimal price)
        {
            course.Price = price;
            return this;
        }

        public CourseBuilder WithDurationHours(int durationHours)
        {
            course.DurationHours = durationHours;
            return this;
        }

        public Course Build() => course;
    }
}
