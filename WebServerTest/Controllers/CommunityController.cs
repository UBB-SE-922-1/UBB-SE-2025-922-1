using Microsoft.AspNetCore.Mvc;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Services.Interfaces;
using WebServerTest.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Duo.Services.Interfaces;
using System.Linq;
using System;
using DuolingoClassLibrary.Repositories.Interfaces;

namespace WebServerTest.Controllers
{
    public class CommunityController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;
        private readonly IHashtagService _hashtagService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly ICommentRepository _commentRepository;
        private const int ItemsPerPage = 10;

        public CommunityController(
            IPostService postService, 
            ICategoryService categoryService, 
            IHashtagService hashtagService,
            ICommentService commentService,
            IUserService userService,
            ICommentRepository commentRepository)
        {
            _postService = postService;
            _categoryService = categoryService;
            _hashtagService = hashtagService;
            _commentService = commentService;
            _userService = userService;
            _commentRepository = commentRepository;
        }

        public async Task<IActionResult> Index(int? categoryId = null, string[] hashtags = null, int page = 1)
        {
            var viewModel = new CommunityViewModel
            {
                CurrentPage = page,
                ItemsPerPage = ItemsPerPage
            };

            // Get all hashtags for the filter
            viewModel.AllHashtags = await _hashtagService.GetAllHashtags();

            // Prepare hashtag list for filtering
            var selectedHashtags = new List<string>();
            if (hashtags != null && hashtags.Length > 0)
            {
                selectedHashtags.AddRange(hashtags.Where(h => !string.IsNullOrEmpty(h)));
                viewModel.SelectedHashtags = selectedHashtags;
            }

            // Get filtered posts
            var (posts, totalCount) = await _postService.GetFilteredAndFormattedPosts(
                categoryId,
                selectedHashtags,
                null, // No text filter
                page,
                ItemsPerPage
            );

            viewModel.Posts = posts;
            viewModel.TotalPosts = totalCount;
            viewModel.SelectedCategoryId = categoryId;
            viewModel.Categories = await _categoryService.GetAllCategories();
            viewModel.TotalPages = (int)Math.Ceiling(viewModel.TotalPosts / (double)ItemsPerPage);

            return View(viewModel);
        }

        public async Task<IActionResult> Post(int id)
        {
            var post = await _postService.GetPostDetailsWithMetadata(id);
            if (post == null)
            {
                return NotFound();
            }

            // Load hashtags for the post
            var postHashtags = await _hashtagService.GetHashtagsByPostId(id);
            post.Hashtags = postHashtags.Select(h => h.Tag).ToList();

            // Load comments for the post
            var comments = await _commentService.GetCommentsByPostId(id);
            ViewBag.Comments = comments;
            ViewBag.PostId = id;

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(string content, int postId, int? parentCommentId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    TempData["Error"] = "Comment content cannot be empty";
                    return RedirectToAction("Post", new { id = postId });
                }

                // Get the current user from session
                var userId = HttpContext.Session.GetInt32("UserId");
                var username = HttpContext.Session.GetString("Username");

                if (userId == null || string.IsNullOrEmpty(username))
                {
                    TempData["Error"] = "You must be logged in to comment";
                    return RedirectToAction("Post", new { id = postId });
                }

                // Create the comment
                var comment = new Comment
                {
                    Content = content,
                    PostId = postId,
                    UserId = userId.Value,
                    ParentCommentId = parentCommentId,
                    CreatedAt = DateTime.Now,
                    Level = parentCommentId.HasValue ? 2 : 1,
                    Username = username
                };

                // Pass the comment directly to the repository
                var commentId = await _commentRepository.CreateComment(comment);
                if (commentId > 0)
                {
                    TempData["Success"] = "Comment added successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to create comment";
                }

                return RedirectToAction("Post", new { id = postId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Post", new { id = postId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeComment(int id, int postId)
        {
            try
            {
                var success = await _commentService.LikeComment(id);
                if (success)
                {
                    TempData["Success"] = "Comment liked successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to like comment";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Post", new { id = postId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikePost(int id)
        {
            try
            {
                var success = await _postService.LikePost(id);
                if (success)
                {
                    TempData["Success"] = "Post liked successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to like post";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Post", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id, int postId)
        {
            try
            {
                // Get the current user from session
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    TempData["Error"] = "You must be logged in to delete comments";
                    return RedirectToAction("Post", new { id = postId });
                }

                // Get all comments for the post
                var allComments = await _commentService.GetCommentsByPostId(postId);
                
                // Find the comment to delete
                var commentToDelete = allComments.FirstOrDefault(c => c.Id == id);
                if (commentToDelete == null)
                {
                    TempData["Error"] = "Comment not found";
                    return RedirectToAction("Post", new { id = postId });
                }

                // Check if the user is the author of the comment
                if (commentToDelete.UserId != userId.Value)
                {
                    TempData["Error"] = "You can only delete your own comments";
                    return RedirectToAction("Post", new { id = postId });
                }

                // Get all replies to this comment (including nested replies)
                var repliesToDelete = allComments.Where(c => c.ParentCommentId == id).ToList();
                foreach (var reply in repliesToDelete)
                {
                    // Recursively delete all nested replies
                    await DeleteComment(reply.Id, postId);
                }

                // Delete the comment itself
                var success = await _commentService.DeleteComment(id, userId.Value);
                if (success)
                {
                    TempData["Success"] = "Comment and all replies deleted successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to delete comment";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Post", new { id = postId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowReplyForm(int commentId, int postId, int level)
        {
            TempData["ReplyCommentId"] = commentId;
            TempData["ReplyLevel"] = level;
            return RedirectToAction("Post", new { id = postId });
        }

        // GET action for creating a new post
        public async Task<IActionResult> CreatePost()
        {
            var viewModel = new CreateEditPostViewModel
            {
                Categories = await _categoryService.GetAllCategories(),
                Hashtags = new List<string>()
            };
            
            return View(viewModel);
        }

        // POST action for creating a new post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreateEditPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload categories if validation fails
                model.Categories = await _categoryService.GetAllCategories();
                return View(model);
            }

            try
            {
                // Get current user ID from session
                var userId = HttpContext.Session.GetInt32("UserId");
                var username = HttpContext.Session.GetString("Username");

                if (userId == null || string.IsNullOrEmpty(username))
                {
                    TempData["Error"] = "You must be logged in to create a post";
                    return RedirectToAction("Index");
                }

                // Create post entity
                var post = new Post
                {
                    Title = model.Title,
                    Description = model.Content,
                    CategoryID = model.CategoryId,
                    UserID = userId.Value,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Username = username
                };

                // Process hashtags - convert comma-separated string to list
                var hashtags = new List<string>();
                if (!string.IsNullOrEmpty(model.HashtagsString))
                {
                    hashtags = model.HashtagsString
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(h => h.Trim().StartsWith("#") ? h.Trim().Substring(1) : h.Trim())
                        .Distinct()
                        .ToList();
                }

                // Create post with hashtags
                var postId = await _postService.CreatePostWithHashtags(post, hashtags, userId.Value);

                if (postId > 0)
                {
                    TempData["Success"] = "Post created successfully!";
                    return RedirectToAction("Post", new { id = postId });
                }
                else
                {
                    TempData["Error"] = "Failed to create post.";
                    model.Categories = await _categoryService.GetAllCategories();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating post: {ex.Message}";
                model.Categories = await _categoryService.GetAllCategories();
                return View(model);
            }
        }

        // GET action for editing a post
        public async Task<IActionResult> EditPost(int id)
        {
            // Get the post to edit
            var post = await _postService.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            // Check if the current user is the author of the post
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || post.UserID != userId)
            {
                TempData["Error"] = "You can only edit your own posts";
                return RedirectToAction("Post", new { id });
            }

            // Get hashtags for the post
            var hashtags = await _hashtagService.GetHashtagsByPostId(id);
            var hashtagsString = string.Join(',', hashtags.Select(h => h.Tag));

            // Create view model
            var viewModel = new CreateEditPostViewModel
            {
                PostId = post.Id,
                Title = post.Title,
                Content = post.Description,
                CategoryId = post.CategoryID,
                Categories = await _categoryService.GetAllCategories(),
                HashtagsString = hashtagsString,
                Hashtags = hashtags.Select(h => h.Tag).ToList(),
                IsEditing = true
            };

            return View(viewModel);
        }

        // POST action for editing a post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(CreateEditPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload categories if validation fails
                model.Categories = await _categoryService.GetAllCategories();
                model.IsEditing = true;
                return View(model);
            }

            try
            {
                // Get current user ID from session
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    TempData["Error"] = "You must be logged in to edit a post";
                    return RedirectToAction("Index");
                }

                // Get existing post
                var post = await _postService.GetPostById(model.PostId);
                if (post == null)
                {
                    return NotFound();
                }

                // Verify post ownership
                var isOwner = await _postService.ValidatePostOwnership(userId.Value, post.Id);
                if (!isOwner)
                {
                    TempData["Error"] = "You can only edit your own posts";
                    return RedirectToAction("Post", new { id = model.PostId });
                }

                // Don't allow changing the category
                if (post.CategoryID != model.CategoryId)
                {
                    TempData["Error"] = "Changing the post's community/category is not allowed";
                    model.CategoryId = post.CategoryID;
                    model.Categories = await _categoryService.GetAllCategories();
                    model.IsEditing = true;
                    return View(model);
                }

                // Update post properties
                post.Title = model.Title;
                post.Description = model.Content;
                post.UpdatedAt = DateTime.Now;

                // Process hashtags - convert comma-separated string to list
                var newHashtags = new List<string>();
                if (!string.IsNullOrEmpty(model.HashtagsString))
                {
                    newHashtags = model.HashtagsString
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(h => h.Trim().StartsWith("#") ? h.Trim().Substring(1) : h.Trim())
                        .Distinct()
                        .ToList();
                }

                // Get current hashtags
                var currentHashtags = await _hashtagService.GetHashtagsByPostId(post.Id);
                var currentHashtagStrings = currentHashtags.Select(h => h.Tag).ToList();

                // Update post
                await _postService.UpdatePost(post);

                // Handle hashtags: remove ones not in the new list, add new ones
                foreach (var hashtag in currentHashtagStrings)
                {
                    if (!newHashtags.Contains(hashtag))
                    {
                        // Find hashtag ID
                        var hashtagToRemove = currentHashtags.First(h => h.Tag == hashtag);
                        await _postService.RemoveHashtagFromPost(post.Id, hashtagToRemove.Id, userId.Value);
                    }
                }

                foreach (var hashtag in newHashtags)
                {
                    if (!currentHashtagStrings.Contains(hashtag))
                    {
                        await _postService.AddHashtagToPost(post.Id, hashtag, userId.Value);
                    }
                }

                TempData["Success"] = "Post updated successfully!";
                return RedirectToAction("Post", new { id = model.PostId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating post: {ex.Message}";
                model.Categories = await _categoryService.GetAllCategories();
                model.IsEditing = true;
                return View(model);
            }
        }
    }
} 