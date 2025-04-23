using Xunit;
using Server.Entities;

namespace TestProject1.Models
{
    public class UserTests
    {
        [Fact]
        public void Constructor_WithUserIdAndUsername_PropertiesAreSet()
        {
            // Arrange
            int expectedUserId = 1;
            string expectedUsername = "TestUser";

            // Act
            var user = new User(expectedUserId, expectedUsername);

            // Assert
            Assert.Equal(expectedUserId, user.UserId);
            Assert.Equal(expectedUsername, user.UserName);
        }

        [Fact]
        public void Constructor_WithOnlyUsername_UserIdDefaultsToZero()
        {
            // Arrange
            string expectedUsername = "TestUser";
            
            // Act
            var user = new User(expectedUsername);

            // Assert
            Assert.Equal(0, user.UserId);
            Assert.Equal(expectedUsername, user.UserName);
        }

        [Fact]
        public void Properties_CanBeModified()
        {
            // Arrange
            var user = new User(1, "OriginalUsername");
            int newUserId = 2;
            string newUsername = "UpdatedUsername";

            // Act
            user.UserId = newUserId;
            user.UserName = newUsername;

            // Assert
            Assert.Equal(newUserId, user.UserId);
            Assert.Equal(newUsername, user.UserName);
        }
    }
} 