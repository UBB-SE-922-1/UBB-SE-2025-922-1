using DuolingoClassLibrary.Entities;
using System.Collections.Generic;

namespace Duo.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public int CreateUser(User user);

        public User GetUserById(int id);

        public User GetUserByUsername(string username);
        /// <summary>
        /// Gets a user by email.
        /// </summary>
        /// <param name="email">The email to search for.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        User GetUserByEmail(string email);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user with updated information.</param>
        void UpdateUser(User user);

        /// <summary>
        /// Validates if the provided credentials are correct.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if credentials are valid; otherwise, false.</returns>
        bool ValidateCredentials(string username, string password);

        /// <summary>
        /// Gets a user by their credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The user if credentials are valid; otherwise, null.</returns>
        User GetUserByCredentials(string username, string password);

        /// <summary>
        /// Gets the top users by number of completed quizzes.
        /// </summary>
        /// <returns>A list of leaderboard entries sorted by quiz completion.</returns>
        List<LeaderboardEntry> GetTopUsersByCompletedQuizzes();

        /// <summary>
        /// Gets the top users by accuracy percentage.
        /// </summary>
        /// <returns>A list of leaderboard entries sorted by accuracy.</returns>
        List<LeaderboardEntry> GetTopUsersByAccuracy();

        /// <summary>
        /// Gets user statistics.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A user object with statistics.</returns>
        User GetUserStats(int userId);

        /// <summary>
        /// Gets all available achievements.
        /// </summary>
        /// <returns>A list of all achievements.</returns>
        List<Achievement> GetAllAchievements();

        /// <summary>
        /// Gets achievements earned by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of achievements earned by the user.</returns>
        List<Achievement> GetUserAchievements(int userId);

        /// <summary>
        /// Awards an achievement to a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="achievementId">The ID of the achievement to award.</param>
        void AwardAchievement(int userId, int achievementId);

        /// <summary>
        /// Gets the friends of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of the user's friends.</returns>
        List<User> GetFriends(int userId);
    }
} 