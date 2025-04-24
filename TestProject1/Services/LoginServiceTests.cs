using System;
using Duo.Interfaces;
using DuolingoClassLibrary.Entities;
using Duo.Repositories.Interfaces;
using Duo.Services;
using Moq;
using Xunit;

namespace TestsDuo2.Services
{
    public class LoginServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly LoginService _loginService;
        
        public LoginServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _loginService = new LoginService(_mockUserRepository.Object);
        }
        
        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new LoginService(null));
        }
        
        [Fact]
        public void AuthenticateUser_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            _mockUserRepository.Setup(r => r.ValidateCredentials(username, password)).Returns(true);
            
            // Act
            bool result = _loginService.AuthenticateUser(username, password);
            
            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.ValidateCredentials(username, password), Times.Once);
        }
        
        [Fact]
        public void AuthenticateUser_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            string username = "testuser";
            string password = "wrongpassword";
            _mockUserRepository.Setup(r => r.ValidateCredentials(username, password)).Returns(false);
            
            // Act
            bool result = _loginService.AuthenticateUser(username, password);
            
            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.ValidateCredentials(username, password), Times.Once);
        }
        
        [Fact]
        public void GetUserByCredentials_ValidCredentials_UpdatesUserStatusAndReturnsUser()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            var user = new User { UserId = 1, UserName = username };
            
            _mockUserRepository.Setup(r => r.ValidateCredentials(username, password)).Returns(true);
            _mockUserRepository.Setup(r => r.GetUserByUsername(username)).Returns(user);
            _mockUserRepository.Setup(r => r.GetUserByCredentials(username, password)).Returns(user);
            
            // Act
            var result = _loginService.GetUserByCredentials(username, password);
            
            // Assert
            Assert.Same(user, result);
            _mockUserRepository.Verify(r => r.ValidateCredentials(username, password), Times.Once);
            _mockUserRepository.Verify(r => r.GetUserByUsername(username), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateUser(It.Is<User>(u => u.OnlineStatus == true)), Times.Once);
            _mockUserRepository.Verify(r => r.GetUserByCredentials(username, password), Times.Once);
        }
        
        [Fact]
        public void GetUserByCredentials_InvalidCredentials_ReturnsUserFromRepository()
        {
            // Arrange
            string username = "testuser";
            string password = "wrongpassword";
            User nullUser = null;
            
            _mockUserRepository.Setup(r => r.ValidateCredentials(username, password)).Returns(false);
            _mockUserRepository.Setup(r => r.GetUserByCredentials(username, password)).Returns(nullUser);
            
            // Act
            var result = _loginService.GetUserByCredentials(username, password);
            
            // Assert
            Assert.Null(result);
            _mockUserRepository.Verify(r => r.ValidateCredentials(username, password), Times.Once);
            _mockUserRepository.Verify(r => r.GetUserByUsername(username), Times.Never);
            _mockUserRepository.Verify(r => r.UpdateUser(It.IsAny<User>()), Times.Never);
            _mockUserRepository.Verify(r => r.GetUserByCredentials(username, password), Times.Once);
        }
        
        [Fact]
        public void UpdateUserStatusOnLogout_ValidUser_UpdatesUserStatus()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "testuser", OnlineStatus = true };
            
            // Act
            _loginService.UpdateUserStatusOnLogout(user);
            
            // Assert
            Assert.False(user.OnlineStatus);
            Assert.NotNull(user.LastActivityDate);
            _mockUserRepository.Verify(r => r.UpdateUser(user), Times.Once);
        }
        
        [Fact]
        public void UpdateUserStatusOnLogout_NullUser_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _loginService.UpdateUserStatusOnLogout(null));
            _mockUserRepository.Verify(r => r.UpdateUser(It.IsAny<User>()), Times.Never);
        }
    }
} 