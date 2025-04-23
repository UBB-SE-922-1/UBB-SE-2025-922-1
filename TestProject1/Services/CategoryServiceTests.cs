using Xunit;
using Moq;
using Duo.Services;
using Server.Entities;
using Server.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace TestProject1.Services
{
    public class CategoryServiceTests
    {
        /*
        [Fact]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CategoryService(null));
        }

        [Fact]
        public void GetCategoryByName_EmptyName_ThrowsArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.GetCategoryByName(string.Empty));
        }

        [Fact]
        public void GetCategoryNames_ReturnsCorrectList()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var list = new List<Category>
            {
                new Category(1, "Category1"),
                new Category(2, "Category2")
            };

            mockRepo.Setup(x => x.GetCategories())
                   .Returns(list);

            var service = new CategoryService(mockRepo.Object);

            // Act
            var result = service.GetCategoryNames();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains("Category1", result);
            Assert.Contains("Category2", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValidCategoryName_NullOrEmpty_ReturnsFalse(string name)
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            // Act
            var result = service.IsValidCategoryName(name);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetAllCategories_ReturnsCategories()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var expectedCategories = new List<Category>
            {
                new Category(1, "Category1"),
                new Category(2, "Category2")
            };
            mockRepo.Setup(x => x.GetCategories())
                   .Returns(expectedCategories);
            var service = new CategoryService(mockRepo.Object);

            // Act
            var result = service.GetAllCategories();

            // Assert
            Assert.Equal(expectedCategories.Count, result.Count);
            Assert.Equal(expectedCategories[0].Name, result[0].Name);
            Assert.Equal(expectedCategories[1].Name, result[1].Name);
        }

        [Fact]
        public void GetAllCategories_HandlesException()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(x => x.GetCategories())
                   .Throws(new Exception("Database error"));
            var service = new CategoryService(mockRepo.Object);

            // Act
            var result = service.GetAllCategories();

            // Assert
            Assert.Empty(result);
        }
    */
    }
       
}