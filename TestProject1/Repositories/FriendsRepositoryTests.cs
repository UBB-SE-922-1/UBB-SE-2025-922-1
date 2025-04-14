using System;
using System.Collections.Generic;
using System.Data;
using Duo.Interfaces;
using Duo.Repositories;
using Microsoft.Data.SqlClient;
using TestsDuo2.Mocks;
using Xunit;

namespace TestsDuo2.Repositories
{
    public class FriendsRepositoryTests
    {
        private readonly MockDataLink mockDataLink;
        private readonly FriendsRepository friendsRepository;

        public FriendsRepositoryTests()
        {
            mockDataLink = new MockDataLink();
            friendsRepository = new FriendsRepository(mockDataLink);
        }
        
        [Fact]
        public void Constructor_WithNullDataLink_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FriendsRepository(null));
        }
        
        [Fact]
        public void DataLink_ReturnsDataLinkInstance()
        {
            // Arrange & Act
            var dataLink = friendsRepository.DataLink;
            
            // Assert
            Assert.Same(mockDataLink, dataLink);
        }
        
        [Fact]
        public void AddFriend_WithValidIds_CallsExecuteNonQuery()
        {
            // Arrange
            int userId = 1;
            int friendId = 2;
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FriendId", friendId)
            };
            mockDataLink.SetupExecuteNonQueryResponse("AddFriend", parameters, 1);

            // Act
            friendsRepository.AddFriend(userId, friendId);

            // Assert
            mockDataLink.VerifyExecuteNonQuery("AddFriend", Times.Once);
        }
        
        [Fact]
        public void GetTopFriendsByCompletedQuizzes_ReturnsLeaderboardEntries()
        {
            // Arrange
            int userId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserId", typeof(int));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("QuizzesCompleted", typeof(int));
            dataTable.Columns.Add("Accuracy", typeof(decimal));
            dataTable.Columns.Add("ProfileImage", typeof(string));
            
            dataTable.Rows.Add(2, "friend1", 50, 98.5m, "profile1.jpg");
            dataTable.Rows.Add(3, "friend2", 45, 97.0m, "profile2.jpg");
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            mockDataLink.SetupExecuteReaderResponse("GetTopFriendsByCompletedQuizzes", parameters, dataTable);

            // Act
            var leaderboard = friendsRepository.GetTopFriendsByCompletedQuizzes(userId);

            // Assert
            Assert.NotNull(leaderboard);
            Assert.Equal(2, leaderboard.Count);
            
            // Check first entry
            Assert.Equal(1, leaderboard[0].Rank);
            Assert.Equal(2, leaderboard[0].UserId);
            Assert.Equal("friend1", leaderboard[0].Username);
            Assert.Equal(50, leaderboard[0].CompletedQuizzes);
            Assert.Equal(98.5m, leaderboard[0].Accuracy);
            
            // Check ranks are assigned correctly
            Assert.Equal(1, leaderboard[0].Rank);
            Assert.Equal(2, leaderboard[1].Rank);
        }
        
        [Fact]
        public void GetTopFriendsByCompletedQuizzes_WithEmptyDataTable_ReturnsEmptyList()
        {
            // Arrange
            int userId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserId", typeof(int));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("QuizzesCompleted", typeof(int));
            dataTable.Columns.Add("Accuracy", typeof(decimal));
            dataTable.Columns.Add("ProfileImage", typeof(string));
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            mockDataLink.SetupExecuteReaderResponse("GetTopFriendsByCompletedQuizzes", parameters, dataTable);

            // Act
            var leaderboard = friendsRepository.GetTopFriendsByCompletedQuizzes(userId);

            // Assert
            Assert.NotNull(leaderboard);
            Assert.Empty(leaderboard);
        }
        
        [Fact]
        public void GetTopFriendsByAccuracy_ReturnsLeaderboardEntries()
        {
            // Arrange
            int userId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserId", typeof(int));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("QuizzesCompleted", typeof(int));
            dataTable.Columns.Add("Accuracy", typeof(decimal));
            dataTable.Columns.Add("ProfileImage", typeof(string));
            
            dataTable.Rows.Add(2, "friend1", 30, 99.5m, "profile1.jpg");
            dataTable.Rows.Add(3, "friend2", 25, 98.0m, "profile2.jpg");
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            mockDataLink.SetupExecuteReaderResponse("GetTopFriendsByAccuracy", parameters, dataTable);

            // Act
            var leaderboard = friendsRepository.GetTopFriendsByAccuracy(userId);

            // Assert
            Assert.NotNull(leaderboard);
            Assert.Equal(2, leaderboard.Count);
            
            // Check first entry
            Assert.Equal(1, leaderboard[0].Rank);
            Assert.Equal(2, leaderboard[0].UserId);
            Assert.Equal("friend1", leaderboard[0].Username);
            Assert.Equal(30, leaderboard[0].CompletedQuizzes);
            Assert.Equal(99.5m, leaderboard[0].Accuracy);
            
            // Check ranks are assigned correctly
            Assert.Equal(1, leaderboard[0].Rank);
            Assert.Equal(2, leaderboard[1].Rank);
        }
        
        [Fact]
        public void GetTopFriendsByAccuracy_WithEmptyDataTable_ReturnsEmptyList()
        {
            // Arrange
            int userId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserId", typeof(int));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("QuizzesCompleted", typeof(int));
            dataTable.Columns.Add("Accuracy", typeof(decimal));
            dataTable.Columns.Add("ProfileImage", typeof(string));
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            mockDataLink.SetupExecuteReaderResponse("GetTopFriendsByAccuracy", parameters, dataTable);

            // Act
            var leaderboard = friendsRepository.GetTopFriendsByAccuracy(userId);

            // Assert
            Assert.NotNull(leaderboard);
            Assert.Empty(leaderboard);
        }
    }
} 