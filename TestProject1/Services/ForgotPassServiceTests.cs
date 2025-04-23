using System;
using System.Threading.Tasks;
using Duo.Constants;
using Duo.Interfaces;
using Server.Entities;
using Duo.Repositories.Interfaces;
using Duo.Services;
using Duo.Services.Interfaces;
using DuolingoNou.Services;
using Moq;
using Xunit;

namespace TestsDuo2.Services
{
    public class ForgotPassServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly ForgotPassService _forgotPassService;

        public ForgotPassServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _forgotPassService = new ForgotPassService(_mockUserRepository.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ForgotPassService(null));
        }

        [Fact]
        public async Task SendVerificationCode_WithValidEmail_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { Email = email };
            _mockUserRepository.Setup(r => r.GetUserByEmail(email)).Returns(user);

            // Act
            var result = await _forgotPassService.SendVerificationCode(email);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.GetUserByEmail(email), Times.Once);
        }

        [Fact]
        public async Task SendVerificationCode_WithInvalidEmail_ReturnsFalse()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _mockUserRepository.Setup(r => r.GetUserByEmail(email)).Returns((User)null);

            // Act
            var result = await _forgotPassService.SendVerificationCode(email);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.GetUserByEmail(email), Times.Once);
        }

        [Fact]
        public async Task SendVerificationCode_GeneratesValidVerificationCode()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { Email = email };
            _mockUserRepository.Setup(r => r.GetUserByEmail(email)).Returns(user);

            // Act
            await _forgotPassService.SendVerificationCode(email);
            var verificationCode = _forgotPassService.GetType().GetField("verificationCode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_forgotPassService) as string;

            // Assert
            Assert.NotNull(verificationCode);
            Assert.True(int.TryParse(verificationCode, out int code));
            Assert.InRange(code, VerificationConstants.MinimumVerificationCodeValue, VerificationConstants.MaximumVerificationCodeValue);
        }

        [Fact]
        public void VerifyCode_WithCorrectCode_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { Email = email };
            _mockUserRepository.Setup(r => r.GetUserByEmail(email)).Returns(user);
            _forgotPassService.SendVerificationCode(email).Wait();
            var verificationCode = _forgotPassService.GetType().GetField("verificationCode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_forgotPassService) as string;

            // Act
            var result = _forgotPassService.VerifyCode(verificationCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyCode_WithIncorrectCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { Email = email };
            _mockUserRepository.Setup(r => r.GetUserByEmail(email)).Returns(user);
            _forgotPassService.SendVerificationCode(email).Wait();

            // Act
            var result = _forgotPassService.VerifyCode("000000");

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void ResetPassword_WithInvalidEmail_ReturnsFalse()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string newPassword = "newPassword123";
            _mockUserRepository.Setup(r => r.GetUserByEmail(email)).Returns((User)null);

            // Act
            var result = _forgotPassService.ResetPassword(email, newPassword);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.GetUserByEmail(email), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateUser(It.IsAny<User>()), Times.Never);
        }
    }
} 