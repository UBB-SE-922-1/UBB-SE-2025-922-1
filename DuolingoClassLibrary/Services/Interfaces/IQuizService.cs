using DuolingoClassLibrary.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuolingoClassLibrary.Services.Interfaces
{
    /// <summary>
    /// Interface for quiz service operations
    /// </summary>
    public interface IQuizService
    {
        /// <summary>
        /// Gets the quizzes completed by the user
        /// </summary>
        /// <returns>A list of completed quizzes</returns>
        Task<List<Quiz>> GetCompletedQuizzesAsync();
    }
} 