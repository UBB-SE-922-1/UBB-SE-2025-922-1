using Server.Entities;
using Duo.Repositories;
using Duo.Interfaces;
using Duo.Constants;
using System;
using System.Collections.Generic;
using Duo.Repositories.Interfaces;

namespace Duo.Services;

public class LeaderboardService
{
    private readonly IUserRepository userRepository;
    private readonly IFriendsRepository friendsRepository;

    public LeaderboardService(IUserRepository userRepository, IFriendsRepository friendsRepository)
    {
        this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        this.friendsRepository = friendsRepository ?? throw new ArgumentNullException(nameof(friendsRepository));
    }

    public List<LeaderboardEntry> GetGlobalLeaderboard(string criteria)
    {
        // Return the top users in the repository sorted by the specified criteria
        if (criteria == LeaderboardConstants.CompletedQuizzesCriteria)
        {
            return userRepository.GetTopUsersByCompletedQuizzes();
        }
        else if (criteria == LeaderboardConstants.AccuracyCriteria)
        {
            return userRepository.GetTopUsersByAccuracy();
        }
        else
        {
            throw new ArgumentException($"Invalid criteria: {criteria}", nameof(criteria));
        }
    }

    public List<LeaderboardEntry> GetFriendsLeaderboard(int userId, string criteria)
    {
        // Return the top friends of the user sorted by the specified criteria
        if (criteria == LeaderboardConstants.CompletedQuizzesCriteria)
        {
            return friendsRepository.GetTopFriendsByCompletedQuizzes(userId);
        }
        else if (criteria == LeaderboardConstants.AccuracyCriteria)
        {
            return friendsRepository.GetTopFriendsByAccuracy(userId);
        }
        else
        {
            throw new ArgumentException($"Invalid criteria: {criteria}", nameof(criteria));
        }
    }

    /// <summary>
    /// Updates the user's score with the specified points.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="points">The points to add.</param>
    public void UpdateUserScore(int userId, int points)
    {
        // TODO: Implement this method
    }

    /// <summary>
    /// Calculates the rank change for a user within the specified time frame.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="timeFrame">The time frame to calculate rank change for.</param>
    public void CalculateRankChange(int userId, string timeFrame)
    {
        // TODO: Implement this method
    }
}

