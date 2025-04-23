using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duo.Constants;
using Server.Entities;

namespace DuolingoNou.Services
{
    /// <summary>
    /// Interface for course service operations
    /// </summary>
    public interface ICourseService
    {
        /// <summary>
        /// Gets the courses the user is enrolled in
        /// </summary>
        /// <returns>A list of enrolled courses</returns>
        Task<List<MyCourse>> GetEnrolledCoursesAsync();
    }

    /// <summary>
    /// Mock implementation of the course service for development
    /// </summary>
    public class MockCourseService : ICourseService
    {
        private static readonly string[] ProgrammingLanguages = new[]
        {
            "Python", "JavaScript", "Java", "C#", "C++",
            "Ruby", "Swift", "Kotlin", "Go", "Rust",
            "TypeScript", "PHP", "Scala", "Dart", "R"
        };

        private static readonly string[] CourseTypes = new[]
        {
            "Beginner", "Intermediate", "Advanced", "Professional",
            "Full Stack", "Web Development", "Mobile Development",
            "Data Science", "Machine Learning", "DevOps"
        };

        /// <summary>
        /// Gets a list of randomly generated mock courses
        /// </summary>
        /// <returns>A list of enrolled courses</returns>
        public async Task<List<MyCourse>> GetEnrolledCoursesAsync()
        {
            await Task.Delay(MockDataConstants.MockAsyncOperationDelayMilliseconds);
            var randomGenerator = new Random();

            // Generate random programming courses
            int courseCount = randomGenerator.Next(
                MockDataConstants.MinimumRandomQuizCount, 
                MockDataConstants.MaximumRandomQuizCount
            );

            return Enumerable.Range(MockDataConstants.MinimumRandomIndex, courseCount)
                .Select(courseIndex => new MyCourse
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"{GetRandomElement(ProgrammingLanguages, randomGenerator)} {GetRandomElement(CourseTypes, randomGenerator)} Course",
                    Language = GetRandomElement(ProgrammingLanguages, randomGenerator),
                    CompletionPercentage = Math.Round(randomGenerator.NextDouble(), MockDataConstants.AccuracyDecimalPlaces),
                    EnrollmentDate = DateTime.Now.AddDays(-randomGenerator.Next(1, MockDataConstants.MaximumCompletionDaysInPast))
                })
                .ToList();
        }

        /// <summary>
        /// Gets a random element from an array
        /// </summary>
        /// <param name="array">The array to select from</param>
        /// <param name="randomGenerator">The random generator to use</param>
        /// <returns>A randomly selected element</returns>
        private string GetRandomElement(string[] array, Random randomGenerator)
        {
            return array[randomGenerator.Next(array.Length)];
        }
    }
}