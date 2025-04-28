using System;
using System.Collections.Generic;
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
    public class CommentServiceTests : IDisposable
    {
        private readonly DataContext _mockContext;
        private readonly CommentRepository _commentRepository;
        private readonly PostRepository _postRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly CommentService _commentService;

        public CommentServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockContext = new DataContext(options);
            _commentRepository = new CommentRepository(_mockContext);
            _postRepository = new PostRepository(_mockContext);
            _mockUserService = new Mock<IUserService>();
            _commentService = new CommentService(_commentRepository, _postRepository, _mockUserService.Object);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Clear existing data
            _mockContext.Comments.RemoveRange(_mockContext.Comments);
            _mockContext.Posts.RemoveRange(_mockContext.Posts);
            _mockContext.SaveChanges();

            // Add test post
            var post = new Post
            {
                Id = 1,
                Title = "Test Post",
                Description = "Test Description",
                UserID = 1,
                CategoryID = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _mockContext.Posts.Add(post);

            // Add test comments
            var comments = new List<Comment>
            {
                new Comment
                {
                    Id = 1,
                    Content = "Test Comment 1",
                    UserId = 1,
                    PostId = 1,
                    ParentCommentId = null,
                    CreatedAt = DateTime.Now,
                    LikeCount = 0,
                    Level = 1
                },
                new Comment
                {
                    Id = 2,
                    Content = "Test Comment 2",
                    UserId = 2,
                    PostId = 1,
                    ParentCommentId = 1,
                    CreatedAt = DateTime.Now,
                    LikeCount = 0,
                    Level = 2
                }
            };
            _mockContext.Comments.AddRange(comments);
            _mockContext.SaveChanges();
        }

        public void Dispose()
        {
            _mockContext.Dispose();
        }

        [Fact]
        public void Constructor_WithNullDependencies_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CommentService(null, _postRepository, _mockUserService.Object));
            Assert.Throws<ArgumentNullException>(() => new CommentService(_commentRepository, null, _mockUserService.Object));
            Assert.Throws<ArgumentNullException>(() => new CommentService(_commentRepository, _postRepository, null));
        }

        [Fact]
        public async Task GetCommentsByPostId_WithValidPostId_ReturnsCommentsWithUsernames()
        {
            // Arrange
            int postId = 1;
            _mockUserService.Setup(s => s.GetUserById(1)).Returns(new User { UserId = 1, UserName = "User1" });
            _mockUserService.Setup(s => s.GetUserById(2)).Returns(new User { UserId = 2, UserName = "User2" });

            // Act
            var comments = await _commentService.GetCommentsByPostId(postId);

            // Assert
            Assert.Equal(2, comments.Count);
            Assert.Equal("User1", comments[0].Username);
            Assert.Equal("User2", comments[1].Username);
        }

        [Fact]
        public async Task GetCommentsByPostId_WithInvalidPostId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _commentService.GetCommentsByPostId(0));
        }

        [Fact]
        public async Task GetProcessedCommentsByPostId_WithValidPostId_ReturnsProcessedComments()
        {
            // Arrange
            int postId = 1;
            _mockUserService.Setup(s => s.GetUserById(It.IsAny<int>())).Returns(new User { UserId = 1, UserName = "TestUser" });

            // Act
            var (allComments, topLevelComments, repliesByParentId) = await _commentService.GetProcessedCommentsByPostId(postId);

            // Assert
            Assert.Equal(2, allComments.Count);
            Assert.Single(topLevelComments);
            Assert.Single(repliesByParentId);
            Assert.Equal(1, topLevelComments[0].Level);
            Assert.Equal(2, repliesByParentId[1][0].Level);
        }

        [Fact]
        public async Task CreateComment_WithValidData_CreatesComment()
        {
            // Arrange
            string content = "New Comment";
            int postId = 1;
            _mockUserService.Setup(s => s.GetCurrentUser()).Returns(new User { UserId = 1, UserName = "TestUser" });

            // Act
            var result = await _commentService.CreateComment(content, postId);

            // Assert
            Assert.True(result > 0);
            var createdComment = await _mockContext.Comments.FindAsync(result);
            Assert.NotNull(createdComment);
            Assert.Equal(content, createdComment.Content);
            Assert.Equal(1, createdComment.Level);
        }

        [Fact]
        public async Task CreateComment_WithInvalidData_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _commentService.CreateComment("", 1));
            await Assert.ThrowsAsync<ArgumentException>(() => _commentService.CreateComment("Test", 0));
        }

        [Fact]
        public async Task CreateComment_WithParentComment_IncrementsLevel()
        {
            // Arrange
            string content = "Reply Comment";
            int postId = 1;
            int parentCommentId = 1;
            _mockUserService.Setup(s => s.GetCurrentUser()).Returns(new User { UserId = 1, UserName = "TestUser" });

            // Act
            var result = await _commentService.CreateComment(content, postId, parentCommentId);

            // Assert
            Assert.True(result > 0);
            var createdComment = await _mockContext.Comments.FindAsync(result);
            Assert.NotNull(createdComment);
            Assert.Equal(2, createdComment.Level);
        }

        [Fact]
        public async Task DeleteComment_WithValidData_DeletesComment()
        {
            // Arrange
            int commentId = 1;
            int userId = 1;
            _mockUserService.Setup(s => s.GetCurrentUser()).Returns(new User { UserId = 1, UserName = "TestUser" });

            // Act
            var result = await _commentService.DeleteComment(commentId, userId);

            // Assert
            Assert.True(result);
            var deletedComment = await _mockContext.Comments.FindAsync(commentId);
            Assert.Null(deletedComment);
        }

        [Fact]
        public async Task DeleteComment_WithInvalidData_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _commentService.DeleteComment(0, 1));
            await Assert.ThrowsAsync<ArgumentException>(() => _commentService.DeleteComment(1, 0));
        }

        [Fact]
        public async Task LikeComment_WithValidCommentId_IncrementsLikeCount()
        {
            // Arrange
            int commentId = 1;

            // Act
            var result = await _commentService.LikeComment(commentId);

            // Assert
            Assert.True(result);
            var comment = await _mockContext.Comments.FindAsync(commentId);
            Assert.Equal(1, comment.LikeCount);
        }

        [Fact]
        public async Task LikeComment_WithInvalidCommentId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _commentService.LikeComment(0));
        }

        [Fact]
        public async Task CreateReplyWithDuplicateCheck_WithValidData_CreatesReply()
        {
            // Arrange
            string replyText = "New Reply";
            int postId = 1;
            int parentCommentId = 1;
            var existingComments = await _commentRepository.GetCommentsByPostId(postId);
            _mockUserService.Setup(s => s.GetCurrentUser()).Returns(new User { UserId = 1, UserName = "TestUser" });

            // Act
            var (success, signature) = await _commentService.CreateReplyWithDuplicateCheck(
                replyText,
                postId,
                parentCommentId,
                existingComments);

            // Assert
            Assert.True(success);
            Assert.NotNull(signature);
        }

        [Fact]
        public async Task CreateReplyWithDuplicateCheck_WithDuplicateContent_ReturnsFalse()
        {
            // Arrange
            string replyText = "Test Comment 2"; // This content already exists
            int postId = 1;
            int parentCommentId = 1;
            var existingComments = await _commentRepository.GetCommentsByPostId(postId);
            _mockUserService.Setup(s => s.GetCurrentUser()).Returns(new User { UserId = 1, UserName = "TestUser" });

            // Act
            var (success, signature) = await _commentService.CreateReplyWithDuplicateCheck(
                replyText,
                postId,
                parentCommentId,
                existingComments);

            // Assert
            Assert.False(success);
            Assert.NotNull(signature);
        }
    }
} 