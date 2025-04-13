using Duo.Data;
using Duo.Models;
using System.Collections.Generic;
using Duo.Interfaces;

namespace Duo.Interfaces
{
    public interface IFriendsRepository
    {
        IDataLink DataLink { get; }
        void AddFriend(int userId, int friendId);
        List<LeaderboardEntry> GetTopFriendsByCompletedQuizzes(int userId);
        List<LeaderboardEntry> GetTopFriendsByAccuracy(int userId);
    }
} 