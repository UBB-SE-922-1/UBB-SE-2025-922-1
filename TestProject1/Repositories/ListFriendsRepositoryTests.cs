using System;
using System.Collections.Generic;
using System.Data;
using Duo.Repositories;
using Microsoft.Data.SqlClient;
using TestsDuo2.Mocks;
using TestsDuo2.TestHelpers;
using Xunit;

namespace TestsDuo2.Repositories
{
    public class ListFriendsRepositoryTests
    {
        private readonly MockDataLink mockDataLink;
        private readonly UserRepository mockUserRepository;
        private readonly ListFriendsRepository listFriendsRepository;
        private readonly List<Duo.Models.User> testFriends;

        public ListFriendsRepositoryTests()
        {
            mockDataLink = new MockDataLink();
            mockUserRepository = new UserRepository(mockDataLink);
            listFriendsRepository = new ListFriendsRepository(mockUserRepository);

            // Set up test friends list
            testFriends = new List<Duo.Models.User>
            {
                UserFactory.CreateUser(
                    id: 1,
                    username: "friend1",
                    dateJoined: new DateTime(2023, 1, 15),
                    onlineStatus: true,
                    lastActivityDate: DateTime.Now.AddMinutes(-5)
                ),
                UserFactory.CreateUser(
                    id: 2,
                    username: "friend2",
                    dateJoined: new DateTime(2023, 2, 20),
                    onlineStatus: false,
                    lastActivityDate: DateTime.Now.AddHours(-2)
                ),
                UserFactory.CreateUser(
                    id: 3,
                    username: "friend3",
                    dateJoined: new DateTime(2022, 12, 10),
                    onlineStatus: false,
                    lastActivityDate: DateTime.Now.AddDays(-1)
                )
            };
        }
        
        [Fact]
        public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ListFriendsRepository(null));
        }
        
        [Fact]
        public void GetFriends_CallsUserRepositoryGetFriends()
        {
            // Arrange
            int userId = 1;
            // Setup a mock response when GetFriends is called on the repository
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserId", typeof(int));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("Password", typeof(string));
            dataTable.Columns.Add("PrivacyStatus", typeof(bool));
            dataTable.Columns.Add("OnlineStatus", typeof(bool));
            dataTable.Columns.Add("DateJoined", typeof(DateTime));
            dataTable.Columns.Add("ProfileImage", typeof(string));
            dataTable.Columns.Add("TotalPoints", typeof(int));
            dataTable.Columns.Add("CoursesCompleted", typeof(int));
            dataTable.Columns.Add("QuizzesCompleted", typeof(int));
            dataTable.Columns.Add("Streak", typeof(int));
            dataTable.Columns.Add("LastActivityDate", typeof(DateTime));
            dataTable.Columns.Add("Accuracy", typeof(decimal));
            
            foreach (var friend in testFriends)
            {
                dataTable.Rows.Add(
                    friend.UserId, 
                    friend.UserName, 
                    "test@example.com", 
                    "password", 
                    false, 
                    friend.OnlineStatus, 
                    friend.DateJoined, 
                    "profile.jpg", 
                    100, 
                    5, 
                    10, 
                    3, 
                    friend.LastActivityDate, 
                    95.5m
                );
            }
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            mockDataLink.SetupExecuteReaderResponse("GetFriends", parameters, dataTable);

            // Act
            var result = listFriendsRepository.GetFriends(userId);

            // Assert
            Assert.Equal(testFriends.Count, result.Count);
            mockDataLink.VerifyExecuteReader("GetFriends", Times.Once);
        }
        
        [Fact]
        public void SortFriendsByName_ReturnsFriendsSortedByName()
        {
            // Arrange
            int userId = 1;
            // Setup a mock response when GetFriends is called on the repository
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserId", typeof(int));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("Password", typeof(string));
            dataTable.Columns.Add("PrivacyStatus", typeof(bool));
            dataTable.Columns.Add("OnlineStatus", typeof(bool));
            dataTable.Columns.Add("DateJoined", typeof(DateTime));
            dataTable.Columns.Add("ProfileImage", typeof(string));
            dataTable.Columns.Add("TotalPoints", typeof(int));
            dataTable.Columns.Add("CoursesCompleted", typeof(int));
            dataTable.Columns.Add("QuizzesCompleted", typeof(int));
            dataTable.Columns.Add("Streak", typeof(int));
            dataTable.Columns.Add("LastActivityDate", typeof(DateTime));
            dataTable.Columns.Add("Accuracy", typeof(decimal));
            
            foreach (var friend in testFriends)
            {
                dataTable.Rows.Add(
                    friend.UserId, 
                    friend.UserName, 
                    "test@example.com", 
                    "password", 
                    false, 
                    friend.OnlineStatus, 
                    friend.DateJoined, 
                    "profile.jpg", 
                    100, 
                    5, 
                    10, 
                    3, 
                    friend.LastActivityDate, 
                    95.5m
                );
            }
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            mockDataLink.SetupExecuteReaderResponse("GetFriends", parameters, dataTable);

            // Act
            var result = listFriendsRepository.SortFriendsByName(userId);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("friend1", result[0].UserName);
            Assert.Equal("friend2", result[1].UserName);
            Assert.Equal("friend3", result[2].UserName);
        }
        
        [Fact]
        public void SortFriendsByDateAdded_ReturnsFriendsSortedByDateJoined()
        {
            // Arrange
            int userId = 1;
            // Setup a mock response when GetFriends is called on the repository
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserId", typeof(int));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("Password", typeof(string));
            dataTable.Columns.Add("PrivacyStatus", typeof(bool));
            dataTable.Columns.Add("OnlineStatus", typeof(bool));
            dataTable.Columns.Add("DateJoined", typeof(DateTime));
            dataTable.Columns.Add("ProfileImage", typeof(string));
            dataTable.Columns.Add("TotalPoints", typeof(int));
            dataTable.Columns.Add("CoursesCompleted", typeof(int));
            dataTable.Columns.Add("QuizzesCompleted", typeof(int));
            dataTable.Columns.Add("Streak", typeof(int));
            dataTable.Columns.Add("LastActivityDate", typeof(DateTime));
            dataTable.Columns.Add("Accuracy", typeof(decimal));
            
            foreach (var friend in testFriends)
            {
                dataTable.Rows.Add(
                    friend.UserId, 
                    friend.UserName, 
                    "test@example.com", 
                    "password", 
                    false, 
                    friend.OnlineStatus, 
                    friend.DateJoined, 
                    "profile.jpg", 
                    100, 
                    5, 
                    10, 
                    3, 
                    friend.LastActivityDate, 
                    95.5m
                );
            }
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            mockDataLink.SetupExecuteReaderResponse("GetFriends", parameters, dataTable);

            // Act
            var result = listFriendsRepository.SortFriendsByDateAdded(userId);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(new DateTime(2022, 12, 10), result[0].DateJoined);
            Assert.Equal(new DateTime(2023, 1, 15), result[1].DateJoined);
            Assert.Equal(new DateTime(2023, 2, 20), result[2].DateJoined);
        }
        
        [Fact]
        public void SortFriendsByOnlineStatus_ReturnsFriendsSortedByOnlineStatusAndLastActivity()
        {
            // Arrange
            int userId = 1;
            // Setup a mock response when GetFriends is called on the repository
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserId", typeof(int));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("Password", typeof(string));
            dataTable.Columns.Add("PrivacyStatus", typeof(bool));
            dataTable.Columns.Add("OnlineStatus", typeof(bool));
            dataTable.Columns.Add("DateJoined", typeof(DateTime));
            dataTable.Columns.Add("ProfileImage", typeof(string));
            dataTable.Columns.Add("TotalPoints", typeof(int));
            dataTable.Columns.Add("CoursesCompleted", typeof(int));
            dataTable.Columns.Add("QuizzesCompleted", typeof(int));
            dataTable.Columns.Add("Streak", typeof(int));
            dataTable.Columns.Add("LastActivityDate", typeof(DateTime));
            dataTable.Columns.Add("Accuracy", typeof(decimal));
            
            foreach (var friend in testFriends)
            {
                dataTable.Rows.Add(
                    friend.UserId, 
                    friend.UserName, 
                    "test@example.com", 
                    "password", 
                    false, 
                    friend.OnlineStatus, 
                    friend.DateJoined, 
                    "profile.jpg", 
                    100, 
                    5, 
                    10, 
                    3, 
                    friend.LastActivityDate, 
                    95.5m
                );
            }
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            mockDataLink.SetupExecuteReaderResponse("GetFriends", parameters, dataTable);

            // Act
            var result = listFriendsRepository.SortFriendsByOnlineStatus(userId);

            // Assert
            Assert.Equal(3, result.Count);
            // First should be the online friend
            Assert.True(result[0].OnlineStatus);
            Assert.Equal(1, result[0].UserId);
            
            // Then offline friends sorted by most recent activity
            Assert.False(result[1].OnlineStatus);
            Assert.Equal(2, result[1].UserId); // friend2 was active 2 hours ago
            
            Assert.False(result[2].OnlineStatus);
            Assert.Equal(3, result[2].UserId); // friend3 was active 1 day ago
        }
    }
} 