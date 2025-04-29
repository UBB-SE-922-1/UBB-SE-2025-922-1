using DuolingoClassLibrary.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Duo.Services.Interfaces
{
    public interface IFriendsService
    {
        Task<List<LeaderboardEntry>> GetTopFriendsByCompletedQuizzes(int userId);
        Task<List<LeaderboardEntry>> GetTopFriendsByAccuracy(int userId);
    }
} 