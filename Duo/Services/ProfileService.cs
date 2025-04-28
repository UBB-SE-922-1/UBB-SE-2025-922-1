using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DuolingoClassLibrary.Entities;
using Duo.Services.Interfaces;

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

        private readonly IUserHelperService _userHelperService;

        public ProfileService(IUserHelperService userHelperService)
        {
            _userHelperService = userHelperService ?? throw new ArgumentNullException(nameof(userHelperService));
        }

        public async Task CreateUser(User userToCreate)
        {
            await _userHelperService.CreateUser(userToCreate);
        }

        public async Task UpdateUser(User userToUpdate)
        {
            await _userHelperService.UpdateUser(userToUpdate);
        }

        public async Task<User> GetUserStats(int userIdentifier)
        {
            return await _userHelperService.GetUserStats(userIdentifier);
        }

        public async Task AwardAchievements(User userToAward)
        {
            List<Achievement> availableAchievements = await _userHelperService.GetAllAchievements();
            List<Achievement> userCurrentAchievements = await _userHelperService.GetUserAchievements(userToAward.UserId);

            foreach (var achievementToCheck in availableAchievements)
            {
                bool hasUserAlreadyEarnedAchievement = userCurrentAchievements.Any(existingAchievement => existingAchievement.Id == achievementToCheck.Id);
                
                if (!hasUserAlreadyEarnedAchievement)
                {
                    if (achievementToCheck.Name.Contains("Streak") && 
                        userToAward.Streak >= CalculateAchievementThreshold(achievementToCheck.Name))
                    {
                        await _userHelperService.AwardAchievement(userToAward.UserId, achievementToCheck.Id);
                        System.Diagnostics.Debug.WriteLine($"Awarded achievement: {achievementToCheck.Name}");
                    }
                    else if (achievementToCheck.Name.Contains("Quizzes Completed") && 
                             userToAward.QuizzesCompleted >= CalculateAchievementThreshold(achievementToCheck.Name))
                    {
                        await _userHelperService.AwardAchievement(userToAward.UserId, achievementToCheck.Id);
                        System.Diagnostics.Debug.WriteLine($"Awarded achievement: {achievementToCheck.Name}");
                    }
                    else if (achievementToCheck.Name.Contains("Courses Completed") && 
                             userToAward.CoursesCompleted >= CalculateAchievementThreshold(achievementToCheck.Name))
                    {
                        await _userHelperService.AwardAchievement(userToAward.UserId, achievementToCheck.Id);
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

        public async Task<List<Achievement>> GetUserAchievements(int userIdentifier)
        {
            return await _userHelperService.GetUserAchievements(userIdentifier);
        }
    }
}
