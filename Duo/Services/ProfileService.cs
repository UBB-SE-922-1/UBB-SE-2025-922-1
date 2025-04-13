using Duo.Models;
using Duo.Repositories;
using Duo.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using Duo.Repositories.Interfaces;

namespace Duo.Services
{
    public class ProfileService
    {
        // Constants for achievement thresholds
        private const int BASIC_ACHIEVEMENT_THRESHOLD = 10;
        private const int BRONZE_ACHIEVEMENT_THRESHOLD = 50;
        private const int SILVER_ACHIEVEMENT_THRESHOLD = 100;
        private const int GOLD_ACHIEVEMENT_THRESHOLD = 250;
        private const int PLATINUM_ACHIEVEMENT_THRESHOLD = 500;
        private const int DIAMOND_ACHIEVEMENT_THRESHOLD = 1000;

        private readonly IUserRepository _userRepositoryService;

        public ProfileService(IUserRepository userRepository)
        {
            _userRepositoryService = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public void CreateUser(User userToCreate)
        {
            _userRepositoryService.CreateUser(userToCreate);
        }

        public void UpdateUser(User userToUpdate)
        {
            _userRepositoryService.UpdateUser(userToUpdate);
        }

        public User GetUserStats(int userIdentifier)
        {
            return _userRepositoryService.GetUserStats(userIdentifier);
        }

        public void AwardAchievements(User userToAward)
        {
            List<Achievement> availableAchievements = _userRepositoryService.GetAllAchievements();
            List<Achievement> userCurrentAchievements = _userRepositoryService.GetUserAchievements(userToAward.UserId);

            foreach (var achievementToCheck in availableAchievements)
            {
                bool hasUserAlreadyEarnedAchievement = userCurrentAchievements.Any(existingAchievement => existingAchievement.Id == achievementToCheck.Id);
                
                if (!hasUserAlreadyEarnedAchievement)
                {
                    if (achievementToCheck.Name.Contains("Streak") && 
                        userToAward.Streak >= CalculateAchievementThreshold(achievementToCheck.Name))
                    {
                        _userRepositoryService.AwardAchievement(userToAward.UserId, achievementToCheck.Id);
                        System.Diagnostics.Debug.WriteLine($"Awarded achievement: {achievementToCheck.Name}");
                    }
                    else if (achievementToCheck.Name.Contains("Quizzes Completed") && 
                             userToAward.QuizzesCompleted >= CalculateAchievementThreshold(achievementToCheck.Name))
                    {
                        _userRepositoryService.AwardAchievement(userToAward.UserId, achievementToCheck.Id);
                        System.Diagnostics.Debug.WriteLine($"Awarded achievement: {achievementToCheck.Name}");
                    }
                    else if (achievementToCheck.Name.Contains("Courses Completed") && 
                             userToAward.CoursesCompleted >= CalculateAchievementThreshold(achievementToCheck.Name))
                    {
                        _userRepositoryService.AwardAchievement(userToAward.UserId, achievementToCheck.Id);
                        System.Diagnostics.Debug.WriteLine($"Awarded achievement: {achievementToCheck.Name}");
                    }
                }
            }
        }

        private int CalculateAchievementThreshold(string achievementName)
        {
            if (achievementName.Contains("10")) return BASIC_ACHIEVEMENT_THRESHOLD;
            if (achievementName.Contains("50")) return BRONZE_ACHIEVEMENT_THRESHOLD;
            if (achievementName.Contains("100")) return SILVER_ACHIEVEMENT_THRESHOLD;
            if (achievementName.Contains("250")) return GOLD_ACHIEVEMENT_THRESHOLD;
            if (achievementName.Contains("500")) return PLATINUM_ACHIEVEMENT_THRESHOLD;
            if (achievementName.Contains("1000")) return DIAMOND_ACHIEVEMENT_THRESHOLD;
            return 0;
        }

        public List<Achievement> GetUserAchievements(int userIdentifier)
        {
            return _userRepositoryService.GetUserAchievements(userIdentifier);
        }
    }
}
