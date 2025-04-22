using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Server.Entities;
using Duo.Data;
using Duo.Repositories.Interfaces;
using Duo.Helpers;
using Duo.Interfaces;

namespace Duo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataLink dataLink;
        // Database error constants
        private const string ERROR_USER_BY_USERNAME = "Database error when getting user by username: ";
        private const string ERROR_USER_BY_EMAIL = "Database error when getting user by email: ";

        // Validation constants
        private const string INVALID_USERNAME_ERROR = "Invalid username.";
        private const string INVALID_EMAIL_ERROR = "Invalid email.";

        // Database stored procedure names
        private const string PROCEDURE_GET_USER_BY_USERNAME = "GetUserByUsername";
        private const string PROCEDURE_GET_USER_BY_EMAIL = "GetUserByEmail";
        private const string PROCEDURE_CREATE_USER = "CreateUser";
        private const string PROCEDURE_UPDATE_USER = "UpdateUser";

        // Database parameter names
        private const string PARAM_USERNAME = "@Username";
        private const string PARAM_EMAIL = "@Email";
        private const string PARAM_USER_ID = "@UserId";
        private const string PARAM_PASSWORD = "@Password";

        // Default user ID for errors
        private const int DEFAULT_ERROR_USER_ID = -1;

        // Database procedure constants for achievement operations
        private const string PROCEDURE_GET_ALL_ACHIEVEMENTS = "GetAllAchievements";
        private const string PROCEDURE_GET_USER_ACHIEVEMENTS = "GetUserAchievements";
        private const string PROCEDURE_AWARD_ACHIEVEMENT = "AwardAchievement";

        // More database procedure constants
        private const string PROCEDURE_GET_USER_STATS = "GetUserStats";
        private const string PROCEDURE_GET_FRIENDS = "GetFriends";

        // Leaderboard procedure constants
        private const string PROCEDURE_GET_TOP_QUIZZES = "GetTopUsersByCompletedQuizzes";
        private const string PROCEDURE_GET_TOP_ACCURACY = "GetTopUsersByAccuracy";

        // Rank constant
        private const int INITIAL_RANK = 1;

        public UserRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public User GetUserById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", id)
            };

            DataTable? dataTable = null;
            try
            {
                dataTable = dataLink.ExecuteReader("GetUserByID", parameters);
                if (dataTable.Rows.Count == 0)
                {
                    throw new Exception("User not found.");
                }
                var row = dataTable.Rows[0];
                return new User(
                    Convert.ToInt32(row[0]),
                    row[1]?.ToString() ?? string.Empty
                );
            }
            finally
            {
                dataTable?.Dispose();
            }
        }

        /// <summary>
        /// Gets a user by username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public User GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException(INVALID_USERNAME_ERROR, nameof(username));
            }

            SqlParameter[] queryParameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_USERNAME, username)
            };

            DataTable? resultDataTable = null;
            try
            {
                resultDataTable = dataLink.ExecuteReader(PROCEDURE_GET_USER_BY_USERNAME, queryParameters);
                if (resultDataTable.Rows.Count == 0)
                {
                    return null;
                }

                return Mappers.MapUser(resultDataTable.Rows[0]);
            }
            catch (SqlException databaseException)
            {
                throw new Exception($"{ERROR_USER_BY_USERNAME}{databaseException.Message}", databaseException);
            }
            finally
            {
                resultDataTable?.Dispose();
            }
        }

        /// <summary>
        /// Gets a user by email.
        /// </summary>
        /// <param name="email">The email to search for.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(INVALID_EMAIL_ERROR, nameof(email));
            }

            SqlParameter[] queryParameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_EMAIL, email)
            };

            DataTable? resultDataTable = null;
            try
            {
                resultDataTable = dataLink.ExecuteReader(PROCEDURE_GET_USER_BY_EMAIL, queryParameters);
                if (resultDataTable.Rows.Count == 0)
                {
                    return null;
                }

                return Mappers.MapUser(resultDataTable.Rows[0]);
            }
            catch (SqlException databaseException)
            {
                throw new Exception($"{ERROR_USER_BY_EMAIL}{databaseException.Message}", databaseException);
            }
            finally
            {
                resultDataTable?.Dispose();
            }
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>The ID of the newly created user.</returns>
        public int CreateUser(User newUserToCreate)
        {
            if (newUserToCreate == null)
            {
                throw new ArgumentNullException(nameof(newUserToCreate));
            }

            SqlParameter[] userCreationParameters = new SqlParameter[]
            {
                new SqlParameter("@UserName", newUserToCreate.UserName),
                new SqlParameter("@Email", newUserToCreate.Email),
                new SqlParameter("@Password", newUserToCreate.Password),
                new SqlParameter("@PrivacyStatus", newUserToCreate.PrivacyStatus),
                new SqlParameter("@OnlineStatus", newUserToCreate.OnlineStatus),
                new SqlParameter("@DateJoined", newUserToCreate.DateJoined),
                new SqlParameter("@ProfileImage", newUserToCreate.ProfileImage ?? string.Empty),
                new SqlParameter("@TotalPoints", newUserToCreate.TotalPoints),
                new SqlParameter("@CoursesCompleted", newUserToCreate.CoursesCompleted),
                new SqlParameter("@QuizzesCompleted", newUserToCreate.QuizzesCompleted),
                new SqlParameter("@Streak", newUserToCreate.Streak),
                new SqlParameter("@LastActivityDate", newUserToCreate.LastActivityDate ?? (object)DBNull.Value),
                new SqlParameter("@Accuracy", newUserToCreate.Accuracy)
            };

            // Use ExecuteScalar to return the newly inserted UserId
            object creationResult = dataLink.ExecuteScalar<int>(PROCEDURE_CREATE_USER, userCreationParameters);

            // Convert result to int (handle null safety)
            return creationResult == null ? DEFAULT_ERROR_USER_ID : Convert.ToInt32(creationResult);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user with updated information.</param>
        public void UpdateUser(User userToUpdate)
        {
            if (userToUpdate == null)
            {
                throw new ArgumentNullException(nameof(userToUpdate));
            }

            SqlParameter[] userUpdateParameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_USER_ID, userToUpdate.UserId),
                new SqlParameter("@UserName", userToUpdate.UserName),
                new SqlParameter("@Email", userToUpdate.Email),
                new SqlParameter(PARAM_PASSWORD, userToUpdate.Password),
                new SqlParameter("@PrivacyStatus", userToUpdate.PrivacyStatus),
                new SqlParameter("@OnlineStatus", userToUpdate.OnlineStatus),
                new SqlParameter("@DateJoined", userToUpdate.DateJoined),
                new SqlParameter("@ProfileImage", userToUpdate.ProfileImage ?? string.Empty),
                new SqlParameter("@TotalPoints", userToUpdate.TotalPoints),
                new SqlParameter("@CoursesCompleted", userToUpdate.CoursesCompleted),
                new SqlParameter("@QuizzesCompleted", userToUpdate.QuizzesCompleted),
                new SqlParameter("@Streak", userToUpdate.Streak),
                new SqlParameter("@LastActivityDate", userToUpdate.LastActivityDate ?? (object)DBNull.Value),
                new SqlParameter("@Accuracy", userToUpdate.Accuracy)
            };

            dataLink.ExecuteNonQuery(PROCEDURE_UPDATE_USER, userUpdateParameters);
        }

        /// <summary>
        /// Validates if the provided credentials are correct.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if credentials are valid; otherwise, false.</returns>
        public bool ValidateCredentials(string username, string password)
        {
            User? retrievedUser = GetUserByUsername(username);
            return retrievedUser != null && retrievedUser.Password == password;
        }

        /// <summary>
        /// Gets a user by their credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The user if credentials are valid; otherwise, null.</returns>
        public User GetUserByCredentials(string username, string password)
        {
            User? retrievedUserAccount = GetUserByUsername(username);
            if (retrievedUserAccount != null && retrievedUserAccount.Password == password)
            {
                return retrievedUserAccount;
            }

            return null; // Either user not found or password doesn't match
        }

        /// <summary>
        /// Gets the top users by completed quizzes for the leaderboard.
        /// </summary>
        /// <returns>A list of users with their rank and score for the leaderboard.</returns>
        public List<LeaderboardEntry> GetTopUsersByCompletedQuizzes()
        {
            var leaderboardDataTable = dataLink.ExecuteReader(PROCEDURE_GET_TOP_QUIZZES);
            List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();
            int currentRank = INITIAL_RANK;

            foreach (DataRow leaderboardRow in leaderboardDataTable.Rows)
            {
                leaderboardEntries.Add(new LeaderboardEntry
                {
                    Rank = currentRank++,
                    UserId = Convert.ToInt32(leaderboardRow["UserId"]),
                    Username = leaderboardRow["UserName"].ToString()!,
                    CompletedQuizzes = Convert.ToInt32(leaderboardRow["QuizzesCompleted"]),
                    Accuracy = Convert.ToDecimal(leaderboardRow["Accuracy"]),
                    ProfilePicture = leaderboardRow["ProfileImage"].ToString()!
                });
            }

            return leaderboardEntries;
        }

        /// <summary>
        /// Gets the top users by accuracy for the leaderboard.
        /// </summary>
        /// <returns>A list of users with their rank and score for the leaderboard.</returns>
        public List<LeaderboardEntry> GetTopUsersByAccuracy()
        {
            var leaderboardAccuracyDataTable = dataLink.ExecuteReader(PROCEDURE_GET_TOP_ACCURACY);
            List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();
            int currentRank = INITIAL_RANK;

            foreach (DataRow leaderboardRow in leaderboardAccuracyDataTable.Rows)
            {
                leaderboardEntries.Add(new LeaderboardEntry
                {
                    Rank = currentRank++,
                    UserId = Convert.ToInt32(leaderboardRow["UserId"]),
                    Username = leaderboardRow["UserName"].ToString()!,
                    CompletedQuizzes = Convert.ToInt32(leaderboardRow["QuizzesCompleted"]),
                    Accuracy = Convert.ToDecimal(leaderboardRow["Accuracy"]),
                    ProfilePicture = leaderboardRow["ProfileImage"].ToString()!
                });
            }

            return leaderboardEntries;
        }

        /// <summary>
        /// Gets user statistics.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user with their statistics.</returns>
        public User GetUserStats(int userId)
        {
            SqlParameter[] userStatsParameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_USER_ID, userId)
            };

            DataTable userStatsDataTable = dataLink.ExecuteReader(PROCEDURE_GET_USER_STATS, userStatsParameters);

            if (userStatsDataTable.Rows.Count > 0)
            {
                return Mappers.MapUser(userStatsDataTable.Rows[0]);
            }

            return null;
        }

        /// <summary>
        /// Gets all available achievements.
        /// </summary>
        /// <returns>A list of all achievements.</returns>
        public List<Achievement> GetAllAchievements()
        {
            DataTable achievementsDataTable = dataLink.ExecuteReader(PROCEDURE_GET_ALL_ACHIEVEMENTS);

            List<Achievement> availableAchievements = new List<Achievement>();
            foreach (DataRow achievementRow in achievementsDataTable.Rows)
            {
                availableAchievements.Add(new Achievement
                {
                    Id = Convert.ToInt32(achievementRow["Id"]),
                    Name = achievementRow["Name"].ToString()!,
                    Description = achievementRow["Description"].ToString()!,
                    RarityLevel = achievementRow["Rarity"].ToString()!
                });
            }

            return availableAchievements;
        }

        /// <summary>
        /// Gets the achievements of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of the user's achievements.</returns>
        public List<Achievement> GetUserAchievements(int userId)
        {
            SqlParameter[] achievementQueryParameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_USER_ID, userId)
            };

            DataTable userAchievementsDataTable = dataLink.ExecuteReader(PROCEDURE_GET_USER_ACHIEVEMENTS, achievementQueryParameters);

            List<Achievement> userAchievementsList = new List<Achievement>();
            foreach (DataRow achievementRow in userAchievementsDataTable.Rows)
            {
                userAchievementsList.Add(new Achievement
                {
                    Id = Convert.ToInt32(achievementRow["AchievementId"]),
                    Name = achievementRow["Name"].ToString()!,
                    Description = achievementRow["Description"].ToString()!,
                    RarityLevel = achievementRow["Rarity"].ToString()!,
                    AchievementUnlockDate = Convert.ToDateTime(achievementRow["AwardedDate"])
                });
            }

            return userAchievementsList;
        }

        /// <summary>
        /// Awards an achievement to a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="achievementId">The ID of the achievement to award.</param>
        public void AwardAchievement(int userId, int achievementId)
        {
            SqlParameter[] awardAchievementParameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_USER_ID, userId),
                new SqlParameter("@AchievementId", achievementId),
                new SqlParameter("@AwardedDate", DateTime.Now)
            };

            dataLink.ExecuteNonQuery(PROCEDURE_AWARD_ACHIEVEMENT, awardAchievementParameters);
        }

        /// <summary>
        /// Gets the friends of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of the user's friends.</returns>
        public List<User> GetFriends(int userId)
        {
            SqlParameter[] friendsQueryParameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_USER_ID, userId)
            };

            DataTable friendsDataTable = dataLink.ExecuteReader(PROCEDURE_GET_FRIENDS, friendsQueryParameters);

            List<User> userFriendsList = new List<User>();
            foreach (DataRow friendRow in friendsDataTable.Rows)
            {
                userFriendsList.Add(Mappers.MapUser(friendRow));
            }

            return userFriendsList;
        }
    }
}

