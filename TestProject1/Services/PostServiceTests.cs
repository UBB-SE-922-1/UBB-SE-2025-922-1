using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DuolingoClassLibrary.Data;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Repos;
using Duo.Services;
using Duo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestProject1.Services
{
    public class PostServiceTests : IDisposable
    {
        private readonly DataContext _mockContext;
        private readonly PostRepository _postRepository;
        private readonly Mock<IHashtagService> _mockHashtagService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ISearchService> _mockSearchService;
        private readonly PostService _postService;

        // Test data
        private const int VALID_POST_ID = 1;
        private const int INVALID_POST_ID = 0;
        private const int VALID_USER_ID = 1;
        private const int VALID_CATEGORY_ID = 1;
        private const int VALID_HASHTAG_ID = 1;
        private const int VALID_PAGE_NUMBER = 1;
        private const int VALID_PAGE_SIZE = 10;
        private const string BASE_QUERY = "Test";
        private const double DEFAULT_SIMILARITY_THRESHOLD = 0.8;

        public PostServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockContext = new DataContext(options);
            _postRepository = new PostRepository(_mockContext);
            _mockHashtagService = new Mock<IHashtagService>();
            _mockUserService = new Mock<IUserService>();
            _mockSearchService = new Mock<ISearchService>();
            _postService = new PostService(_postRepository, _mockHashtagService.Object, _mockUserService.Object, _mockSearchService.Object);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Clear existing data
            _mockContext.Posts.RemoveRange(_mockContext.Posts);
            _mockContext.SaveChanges();

            // Add test posts
            var posts = new List<Post>
            {
                new Post
                {
                    Id = VALID_POST_ID,
                    Title = "Test Post 1",
                    Description = "Test Description 1",
                    UserID = VALID_USER_ID,
                    CategoryID = VALID_CATEGORY_ID,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LikeCount = 0
                },
                new Post
                {
                    Id = 2,
                    Title = "Test Post 2",
                    Description = "Test Description 2",
                    UserID = 2,
                    CategoryID = VALID_CATEGORY_ID,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LikeCount = 0
                }
            };
            _mockContext.Posts.AddRange(posts);
            _mockContext.SaveChanges();
        }

        public void Dispose()
        {
            _mockContext.Dispose();
        }

        [Fact]
        public void Constructor_WithNullDependencies_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PostService(null, _mockHashtagService.Object, _mockUserService.Object, _mockSearchService.Object));
            Assert.Throws<ArgumentNullException>(() => new PostService(_postRepository, null, _mockUserService.Object, _mockSearchService.Object));
            Assert.Throws<ArgumentNullException>(() => new PostService(_postRepository, _mockHashtagService.Object, null, _mockSearchService.Object));
            Assert.Throws<ArgumentNullException>(() => new PostService(_postRepository, _mockHashtagService.Object, _mockUserService.Object, null));
        }

        [Fact]
        public async Task CreatePost_WithValidData_CreatesPost()
        {
            // Arrange
            var newPost = new Post
            {
                Title = "New Post",
                Description = "New Description",
                UserID = VALID_USER_ID,
                CategoryID = VALID_CATEGORY_ID
            };

            // Act
            var result = await _postService.CreatePost(newPost);

            // Assert
            Assert.True(result > 0);
            var createdPost = await _mockContext.Posts.FindAsync(result);
            Assert.NotNull(createdPost);
            Assert.Equal(newPost.Title, createdPost.Title);
            Assert.Equal(newPost.Description, createdPost.Description);
        }

        [Fact]
        public async Task CreatePost_WithInvalidData_ThrowsArgumentException()
        {
            // Arrange
            var invalidPost = new Post
            {
                Title = "",
                Description = "Test Description",
                UserID = VALID_USER_ID,
                CategoryID = VALID_CATEGORY_ID
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _postService.CreatePost(invalidPost));
        }

        [Fact]
        public async Task DeletePost_WithValidId_DeletesPost()
        {
            // Act
            await _postService.DeletePost(VALID_POST_ID);

            // Assert
            var deletedPost = await _mockContext.Posts.FindAsync(VALID_POST_ID);
            Assert.Null(deletedPost);
        }

        [Fact]
        public async Task DeletePost_WithInvalidId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _postService.DeletePost(INVALID_POST_ID));
        }

        [Fact]
        public async Task UpdatePost_WithValidData_UpdatesPost()
        {
            // Arrange
            var post = await _mockContext.Posts.FindAsync(VALID_POST_ID);
            post.Title = "Updated Title";
            post.Description = "Updated Description";

            // Act
            await _postService.UpdatePost(post);

            // Assert
            var updatedPost = await _mockContext.Posts.FindAsync(VALID_POST_ID);
            Assert.Equal("Updated Title", updatedPost.Title);
            Assert.Equal("Updated Description", updatedPost.Description);
        }

        [Fact]
        public async Task GetPostById_WithValidId_ReturnsPost()
        {
            // Act
            var result = await _postService.GetPostById(VALID_POST_ID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(VALID_POST_ID, result.Id);
        }

        [Fact]
        public async Task GetPostById_WithInvalidId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _postService.GetPostById(INVALID_POST_ID));
        }

        [Fact]
        public async Task GetPostsByCategory_WithValidData_ReturnsPosts()
        {
            // Act
            var result = await _postService.GetPostsByCategory(VALID_CATEGORY_ID, VALID_PAGE_NUMBER, VALID_PAGE_SIZE);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetPaginatedPosts_WithValidData_ReturnsPosts()
        {
            // Act
            var result = await _postService.GetPaginatedPosts(VALID_PAGE_NUMBER, VALID_PAGE_SIZE);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetTotalPostCount_ReturnsCorrectCount()
        {
            // Act
            var result = await _postService.GetTotalPostCount();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetPostCountByCategoryId_WithValidId_ReturnsCorrectCount()
        {
            // Act
            var result = await _postService.GetPostCountByCategoryId(VALID_CATEGORY_ID);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task LikePost_WithValidId_IncrementsLikeCount()
        {
            // Act
            var result = await _postService.LikePost(VALID_POST_ID);

            // Assert
            Assert.True(result);
            var post = await _mockContext.Posts.FindAsync(VALID_POST_ID);
            Assert.Equal(1, post.LikeCount);
        }

        [Fact]
        public async Task GetPostDetailsWithMetadata_WithValidId_ReturnsPostWithMetadata()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserById(VALID_USER_ID))
                .Returns(new User { UserId = VALID_USER_ID, UserName = "TestUser" });
            _mockHashtagService.Setup(s => s.GetHashtagsByPostId(VALID_POST_ID))
                .ReturnsAsync(new List<Hashtag>());

            // Act
            var result = await _postService.GetPostDetailsWithMetadata(VALID_POST_ID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestUser", result.Username);
            Assert.NotNull(result.Hashtags);
        }

        [Fact]
        public async Task AddHashtagToPost_WithValidData_AddsHashtag()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetCurrentUser())
                .Returns(new User { UserId = VALID_USER_ID, UserName = "TestUser" });
            _mockHashtagService.Setup(s => s.GetHashtagByText("test"))
                .ReturnsAsync((Hashtag)null);
            _mockHashtagService.Setup(s => s.CreateHashtag("test"))
                .ReturnsAsync(new Hashtag { Id = VALID_HASHTAG_ID, Tag = "test" });
            _mockHashtagService.Setup(s => s.AddHashtagToPost(VALID_POST_ID, VALID_HASHTAG_ID))
                .ReturnsAsync(true);

            // Act
            var result = await _postService.AddHashtagToPost(VALID_POST_ID, "test", VALID_USER_ID);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveHashtagFromPost_WithValidData_RemovesHashtag()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetCurrentUser())
                .Returns(new User { UserId = VALID_USER_ID, UserName = "TestUser" });
            _mockHashtagService.Setup(s => s.RemoveHashtagFromPost(VALID_POST_ID, VALID_HASHTAG_ID))
                .ReturnsAsync(true);

            // Act
            var result = await _postService.RemoveHashtagFromPost(VALID_POST_ID, VALID_HASHTAG_ID, VALID_USER_ID);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreatePostWithHashtags_WithValidData_CreatesPostWithHashtags()
        {
            // Arrange
            var newPost = new Post
            {
                Title = "New Post with Hashtags",
                Description = "New Description",
                UserID = VALID_USER_ID,
                CategoryID = VALID_CATEGORY_ID
            };
            var hashtagList = new List<string> { "test1", "test2" };

            _mockHashtagService.Setup(s => s.GetHashtagByText(It.IsAny<string>()))
                .ReturnsAsync((Hashtag)null);
            _mockHashtagService.Setup(s => s.CreateHashtag(It.IsAny<string>()))
                .ReturnsAsync((string tag) => new Hashtag { Id = VALID_HASHTAG_ID, Tag = tag });
            _mockHashtagService.Setup(s => s.AddHashtagToPost(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var result = await _postService.CreatePostWithHashtags(newPost, hashtagList, VALID_USER_ID);

            // Assert
            Assert.True(result > 0);
            var createdPost = await _mockContext.Posts.FindAsync(result);
            Assert.NotNull(createdPost);
            Assert.Equal(newPost.Title, createdPost.Title);
        }

        [Fact]
        public async Task GetFilteredAndFormattedPosts_WithValidData_ReturnsFilteredPosts()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserById(It.IsAny<int>()))
                .Returns(new User { UserId = VALID_USER_ID, UserName = "TestUser" });
            _mockHashtagService.Setup(s => s.GetHashtagsByPostId(It.IsAny<int>()))
                .ReturnsAsync(new List<Hashtag>());
            _mockSearchService.Setup(s => s.FindFuzzySearchMatches(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<double>()))
                .Returns(new List<string> { "Test Post 1" });

            // Act
            var (posts, totalCount) = await _postService.GetFilteredAndFormattedPosts(
                VALID_CATEGORY_ID,
                new List<string>(),
                BASE_QUERY,
                VALID_PAGE_NUMBER,
                VALID_PAGE_SIZE);

            // Assert
            Assert.NotNull(posts);
            Assert.Equal(2, totalCount);
            Assert.Equal("TestUser", posts[0].Username);
        }
    }
} 