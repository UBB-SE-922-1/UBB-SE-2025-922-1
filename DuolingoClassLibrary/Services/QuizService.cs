using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DuolingoClassLibrary.Constants;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Services.Interfaces;

namespace DuolingoClassLibrary.Services
{
    /// <summary>
    /// Mock implementation of the quiz service for development
    /// </summary>
    public class QuizService : IQuizService
    {
        private static readonly string[] QuizTopics = new[]
        {
            "Syntax", "Data Structures", "Algorithms", "Object-Oriented Programming",
            "Functional Programming", "Memory Management", "Concurrency",
            "Design Patterns", "Web Frameworks", "Database Interactions",
            "Error Handling", "Testing", "Performance Optimization",
            "Security", "Asynchronous Programming"
        };

        private static readonly string[] ProgrammingLanguages = new[]
        {
            "Python", "JavaScript", "Java", "C#", "C++",
            "Ruby", "Swift", "Kotlin", "Go", "Rust",
            "TypeScript", "PHP", "Scala", "Dart", "R"
        };

        private static readonly string[] QuizTypes = new[]
        {
            "Basic", "Challenge", "Comprehensive", "Speed", "Diagnostic"
        };

        /// <summary>
        /// Gets a list of randomly generated mock quizzes
        /// </summary>
        /// <returns>A list of completed quizzes</returns>
        public async Task<List<Quiz>> GetCompletedQuizzesAsync()
        {
            await Task.Delay(MockDataConstants.MockAsyncOperationDelayMilliseconds); // Simulate async operation
            var randomGenerator = new Random();

            // Generate random programming quizzes
            int quizCount = randomGenerator.Next(
                MockDataConstants.MinimumRandomQuizCount, 
                MockDataConstants.MaximumRandomQuizCount
            );

            return Enumerable.Range(MockDataConstants.MinimumRandomIndex, quizCount)
                .Select(quizIndex => new Quiz
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"{GetRandomElement(ProgrammingLanguages, randomGenerator)} {GetRandomElement(QuizTypes, randomGenerator)} {GetRandomElement(QuizTopics, randomGenerator)} Quiz",
                    Category = GetRandomElement(QuizTopics, randomGenerator),
                    AccuracyPercentage = Math.Round(randomGenerator.NextDouble(), MockDataConstants.AccuracyDecimalPlaces), // Random accuracy between 0 and 1
                    CompletionDate = DateTime.Now.AddDays(-randomGenerator.Next(1, MockDataConstants.MaximumCompletionDaysInPast)) // Completed within last year
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