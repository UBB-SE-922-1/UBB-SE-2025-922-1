using System;
using System.Collections.Generic;
using Duo.Constants;
using Duo.Interfaces;
using DuolingoClassLibrary.Entities;
using Duo.Services;
using Moq;
using Xunit;
using Duo.Repositories.Interfaces;

namespace TestsDuo2.Services
{
    public class LeaderboardServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IFriendsLeaderboardRepository> _mockFriendsRepository;
        private readonly LeaderboardService _leaderboardService;
        
        public LeaderboardServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockFriendsRepository = new Mock<IFriendsLeaderboardRepository>();
            _leaderboardService = new LeaderboardService(_mockUserRepository.Object, _mockFriendsRepository.Object);
        }
        
        [Fact]
        public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new LeaderboardService(null, _mockFriendsRepository.Object));
        }
        
        [Fact]
        public void Constructor_WithNullFriendsRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new LeaderboardService(_mockUserRepository.Object, null));
        }
        
        [Fact]
        public void GetGlobalLeaderboard_WithCompletedQuizzesCriteria_ReturnsTopUsers()
        {
            // Arrange
            var expectedEntries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { UserId = 1, Username = "User1", ScoreValue = 100 },
                new LeaderboardEntry { UserId = 2, Username = "User2", ScoreValue = 90 }
            };
            _mockUserRepository.Setup(r => r.GetTopUsersByCompletedQuizzes()).Returns(expectedEntries);
            
            // Act
            var result = _leaderboardService.GetGlobalLeaderboard(LeaderboardConstants.CompletedQuizzesCriteria);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntries.Count, result.Count);
            Assert.Equal(expectedEntries, result);
            _mockUserRepository.Verify(r => r.GetTopUsersByCompletedQuizzes(), Times.Once);
        }
        
        [Fact]
        public void GetGlobalLeaderboard_WithAccuracyCriteria_ReturnsTopUsers()
        {
            // Arrange
            var expectedEntries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { UserId = 1, Username = "User1", ScoreValue = (decimal) 0.95 },
                new LeaderboardEntry { UserId = 2, Username = "User2", ScoreValue = (decimal) 0.90 }
            };
            _mockUserRepository.Setup(r => r.GetTopUsersByAccuracy()).Returns(expectedEntries);
            
            // Act
            var result = _leaderboardService.GetGlobalLeaderboard(LeaderboardConstants.AccuracyCriteria);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntries.Count, result.Count);
            Assert.Equal(expectedEntries, result);
            _mockUserRepository.Verify(r => r.GetTopUsersByAccuracy(), Times.Once);
        }
        
        [Fact]
        public void GetGlobalLeaderboard_WithInvalidCriteria_ThrowsArgumentException()
        {
            // Arrange
            string invalidCriteria = "InvalidCriteria";
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _leaderboardService.GetGlobalLeaderboard(invalidCriteria));
            Assert.Contains("Invalid criteria", exception.Message);
            Assert.Equal("criteria", exception.ParamName);
        }
        
        [Fact]
        public void GetFriendsLeaderboard_WithCompletedQuizzesCriteria_ReturnsTopFriends()
        {
            // Arrange
            int userId = 1;
            var expectedEntries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { UserId = 2, Username = "Friend1", ScoreValue = 100 },
                new LeaderboardEntry { UserId = 3, Username = "Friend2", ScoreValue = 90 }
            };
            _mockFriendsRepository.Setup(r => r.GetTopFriendsByCompletedQuizzes(userId)).Returns(expectedEntries);
            
            // Act
            var result = _leaderboardService.GetFriendsLeaderboard(userId, LeaderboardConstants.CompletedQuizzesCriteria);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntries.Count, result.Count);
            Assert.Equal(expectedEntries, result);
            _mockFriendsRepository.Verify(r => r.GetTopFriendsByCompletedQuizzes(userId), Times.Once);
        }
        
        [Fact]
        public void GetFriendsLeaderboard_WithAccuracyCriteria_ReturnsTopFriends()
        {
            // Arrange
            int userId = 1;
            var expectedEntries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { UserId = 2, Username = "Friend1", ScoreValue = (decimal) 0.95 },
                new LeaderboardEntry { UserId = 3, Username = "Friend2", ScoreValue = (decimal) 0.90 }
            };
            _mockFriendsRepository.Setup(r => r.GetTopFriendsByAccuracy(userId)).Returns(expectedEntries);
            
            // Act
            var result = _leaderboardService.GetFriendsLeaderboard(userId, LeaderboardConstants.AccuracyCriteria);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntries.Count, result.Count);
            Assert.Equal(expectedEntries, result);
            _mockFriendsRepository.Verify(r => r.GetTopFriendsByAccuracy(userId), Times.Once);
        }
        
        [Fact]
        public void GetFriendsLeaderboard_WithInvalidCriteria_ThrowsArgumentException()
        {
            // Arrange
            int userId = 1;
            string invalidCriteria = "InvalidCriteria";
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _leaderboardService.GetFriendsLeaderboard(userId, invalidCriteria));
            Assert.Contains("Invalid criteria", exception.Message);
            Assert.Equal("criteria", exception.ParamName);
        }
        
        [Fact]
        public void GetFriendsLeaderboard_WithInvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            int invalidUserId = -1;
            _mockFriendsRepository.Setup(r => r.GetTopFriendsByCompletedQuizzes(invalidUserId)).Returns(new List<LeaderboardEntry>());
            
            // Act
            var result = _leaderboardService.GetFriendsLeaderboard(invalidUserId, LeaderboardConstants.CompletedQuizzesCriteria);
            
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockFriendsRepository.Verify(r => r.GetTopFriendsByCompletedQuizzes(invalidUserId), Times.Once);
        }
        
        [Fact]
        public void GetGlobalLeaderboard_WithEmptyResults_ReturnsEmptyList()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetTopUsersByCompletedQuizzes()).Returns(new List<LeaderboardEntry>());
            
            // Act
            var result = _leaderboardService.GetGlobalLeaderboard(LeaderboardConstants.CompletedQuizzesCriteria);
            
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockUserRepository.Verify(r => r.GetTopUsersByCompletedQuizzes(), Times.Once);
        }
        
        [Fact]
        public void UpdateUserScore_ExecutesSuccessfully()
        {
            // Arrange
            int userId = 1;
            int points = 10;
            
            // Act - Should not throw
            _leaderboardService.UpdateUserScore(userId, points);
            
            // Assert - Currently this is a TODO method, so nothing to verify
        }
        
        [Fact]
        public void CalculateRankChange_ExecutesSuccessfully()
        {
            // Arrange
            int userId = 1;
            string timeFrame = "weekly";
            
            // Act - Should not throw
            _leaderboardService.CalculateRankChange(userId, timeFrame);
            
            // Assert - Currently this is a TODO method, so nothing to verify
        }
    }
} 