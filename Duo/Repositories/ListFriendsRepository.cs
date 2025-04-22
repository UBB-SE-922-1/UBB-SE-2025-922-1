using Duo.Data;
using Duo.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using Server.Entities;
using System.Linq;

namespace Duo.Repositories
{
    public class ListFriendsRepository
    {
        private readonly UserRepository userRepository;

        public ListFriendsRepository(UserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public List<User> GetFriends(int userId)
        {
            return this.userRepository.GetFriends(userId);
        }

        // Sort friends by name (ascending)
        public List<User> SortFriendsByName(int userId)
        {
            var friends = GetFriends(userId);
            return friends.OrderBy(f => f.UserName).ToList();
        }

        // Sort friends by date added (ascending)
        public List<User> SortFriendsByDateAdded(int userId)
        {
            var friends = GetFriends(userId);
            return friends.OrderBy(f => f.DateJoined).ToList();
        }

        public List<User> SortFriendsByOnlineStatus(int userId)
        {
            var friends = GetFriends(userId);

            // First, order by OnlineStatus: active users (OnlineStatus == 1) will come first
            // Then, for inactive users (OnlineStatus == 0), order by the LastActivityDate, descending (most recent first)
            return friends
                .OrderByDescending(f => f.OnlineStatus) // Active users first
                .ThenByDescending(f => f.LastActivityDate ?? DateTime.MinValue) // Inactive users sorted by LastActivityDate
                .ToList();
        }


    }
}
