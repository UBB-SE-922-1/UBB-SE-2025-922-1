using System;
using System.Threading.Tasks;
using Duo.Interfaces;
using Duo.Models;
using Duo.Repositories.Interfaces;
using Duo.Services;
using Moq;
using Xunit;

namespace TestsDuo2.Services
{
    public class SignUpServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly SignUpService _signUpService;
        
        public SignUpServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _signUpService = new SignUpService(_mockUserRepository.Object);
        }
        
        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SignUpService(null));
        }
        
        [Fact]
        public async Task IsUsernameTaken_WhenUserExists_ReturnsTrue()
        {
            // Arrange
            string username = "existinguser";
            var existingUser = new User { UserId = 1, UserName = username };
            _mockUserRepository.Setup(r => r.GetUserByUsername(username)).Returns(existingUser);
            
            // Act
            bool result = await _signUpService.IsUsernameTaken(username);
            
            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.GetUserByUsername(username), Times.Once);
        }
        
        [Fact]
        public async Task IsUsernameTaken_WhenUserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            string username = "newuser";
            User nullUser = null;
            _mockUserRepository.Setup(r => r.GetUserByUsername(username)).Returns(nullUser);
            
            // Act
            bool result = await _signUpService.IsUsernameTaken(username);
            
            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.GetUserByUsername(username), Times.Once);
        }
        
        [Fact]
        public async Task IsUsernameTaken_WhenExceptionOccurs_ReturnsTrue()
        {
            // Arrange
            string username = "erroruser";
            _mockUserRepository.Setup(r => r.GetUserByUsername(username)).Throws(new Exception("Database error"));
            
            // Act
            bool result = await _signUpService.IsUsernameTaken(username);
            
            // Assert
            Assert.True(result); // Should return true (fail-safe) on exception
            _mockUserRepository.Verify(r => r.GetUserByUsername(username), Times.Once);
        }
        
        [Fact]
        public async Task RegisterUser_WhenEmailExists_ReturnsFalse()
        {
            // Arrange
            var user = new User { UserId = 0, UserName = "newuser", Email = "existing@example.com" };
            var existingUser = new User { UserId = 1, UserName = "existinguser", Email = user.Email };
            
            _mockUserRepository.Setup(r => r.GetUserByEmail(user.Email)).Returns(existingUser);
            
            // Act
            bool result = await _signUpService.RegisterUser(user);
            
            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.GetUserByEmail(user.Email), Times.Once);
            _mockUserRepository.Verify(r => r.GetUserByUsername(It.IsAny<string>()), Times.Never);
            _mockUserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
        }
        
        [Fact]
        public async Task RegisterUser_WhenUsernameExists_ReturnsFalse()
        {
            // Arrange
            var user = new User { UserId = 0, UserName = "existinguser", Email = "new@example.com" };
            var existingUser = new User { UserId = 1, UserName = user.UserName, Email = "existing@example.com" };
            
            User nullUser = null;
            _mockUserRepository.Setup(r => r.GetUserByEmail(user.Email)).Returns(nullUser);
            _mockUserRepository.Setup(r => r.GetUserByUsername(user.UserName)).Returns(existingUser);
            
            // Act
            bool result = await _signUpService.RegisterUser(user);
            
            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.GetUserByEmail(user.Email), Times.Once);
            _mockUserRepository.Verify(r => r.GetUserByUsername(user.UserName), Times.Once);
            _mockUserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
        }
        
        [Fact]
        public async Task RegisterUser_WithValidUser_ReturnsTrue()
        {
            // Arrange
            var user = new User { UserId = 0, UserName = "newuser", Email = "new@example.com" };
            int newUserId = 1;
            
            User nullUser = null;
            _mockUserRepository.Setup(r => r.GetUserByEmail(user.Email)).Returns(nullUser);
            _mockUserRepository.Setup(r => r.GetUserByUsername(user.UserName)).Returns(nullUser);
            _mockUserRepository.Setup(r => r.CreateUser(It.IsAny<User>())).Returns(newUserId);
            
            // Act
            bool result = await _signUpService.RegisterUser(user);
            
            // Assert
            Assert.True(result);
            Assert.True(user.OnlineStatus);
            Assert.Equal(newUserId, user.UserId);
            _mockUserRepository.Verify(r => r.GetUserByEmail(user.Email), Times.Once);
            _mockUserRepository.Verify(r => r.GetUserByUsername(user.UserName), Times.Once);
            _mockUserRepository.Verify(r => r.CreateUser(user), Times.Once);
        }
    }
} 