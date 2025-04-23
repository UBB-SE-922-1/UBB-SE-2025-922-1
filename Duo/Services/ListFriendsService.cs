using Duo;
using Server.Entities;
using Duo.Repositories;
using Duo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Duo.Services
{
    /// <summary>
    /// Service for managing friend relationships between users.
    /// </summary>
    public class FriendsService
    {
        private readonly ListFriendsRepository friendRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendsService"/> class.
        /// </summary>
        /// <param name="friendRepository">The list friends repository.</param>
        public FriendsService(ListFriendsRepository friendRepository)
        {
            this.friendRepository = friendRepository ?? throw new ArgumentNullException(nameof(friendRepository));
        }

        /// <summary>
        /// Gets the friends for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of user's friends.</returns>
        public List<User> GetFriends(int userId)
        {
            return friendRepository.GetFriends(userId);
        }

        /// <summary>
        /// Sorts friends alphabetically by name.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A sorted list of user's friends.</returns>
        public List<User> SortFriendsByName(int userId)
        {
            return friendRepository.SortFriendsByName(userId);
        }

        /// <summary>
        /// Sorts friends by the date they were added.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A sorted list of user's friends.</returns>
        public List<User> SortFriendsByDateAdded(int userId)
        {
            return friendRepository.SortFriendsByDateAdded(userId);
        }

        /// <summary>
        /// Sorts friends by their online status.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A sorted list of user's friends.</returns>
        public List<User> SortFriendsByOnlineStatus(int userId)
        {
            return friendRepository.SortFriendsByOnlineStatus(userId);
        }
    }
}
