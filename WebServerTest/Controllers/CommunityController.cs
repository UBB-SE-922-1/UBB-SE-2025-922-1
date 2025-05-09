using Microsoft.AspNetCore.Mvc;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Services.Interfaces;
using WebServerTest.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Duo.Services.Interfaces;
using System.Linq;

namespace WebServerTest.Controllers
{
    public class CommunityController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;
        private readonly IHashtagService _hashtagService;
        private readonly ICommentService _commentService;
        private const int ItemsPerPage = 10;

        public CommunityController(
            IPostService postService, 
            ICategoryService categoryService, 
            IHashtagService hashtagService,
            ICommentService commentService)
        {
            _postService = postService;
            _categoryService = categoryService;
            _hashtagService = hashtagService;
            _commentService = commentService;
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
        public async Task<IActionResult> AddComment(string content, int postId, int? parentCommentId = null)
        {
            try
            {
                var commentId = await _commentService.CreateComment(content, postId, parentCommentId);
                return Json(new { success = true, commentId = commentId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> LikeComment(int id)
        {
            try
            {
                var success = await _commentService.LikeComment(id);
                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
} 