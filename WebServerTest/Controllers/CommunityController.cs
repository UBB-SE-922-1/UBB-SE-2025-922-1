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
    }
} 