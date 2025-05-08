using DuolingoClassLibrary.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuolingoClassLibrary.Services.Interfaces
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
} 