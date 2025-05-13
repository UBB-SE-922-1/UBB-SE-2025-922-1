using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DuolingoClassLibrary.Data;
using DuolingoClassLibrary.Entities;
using Duolingo.WebServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace Duolingo.WebServer.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly DataContext _context;

        public PostsController(DataContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.PostHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .ToListAsync();

            var viewModels = posts.Select(p => new PostViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                UserID = p.UserID,
                CategoryID = p.CategoryID,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                LikeCount = p.LikeCount,
                Username = p.User?.UserName ?? "Unknown User",
                CategoryName = p.Category?.Name ?? "Uncategorized",
                Hashtags = p.PostHashtags.Select(ph => ph.Hashtag.Name).ToList()
            });

            return View(viewModels);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.PostHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var viewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                UserID = post.UserID,
                CategoryID = post.CategoryID,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikeCount = post.LikeCount,
                Username = post.User?.UserName ?? "Unknown User",
                CategoryName = post.Category?.Name ?? "Uncategorized",
                Hashtags = post.PostHashtags.Select(ph => ph.Hashtag.Name).ToList()
            };

            return View(viewModel);
        }

        // GET: Posts/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new PostViewModel
            {
                AvailableCategories = await _context.Categories.ToListAsync(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return View(viewModel);
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,CategoryID,Hashtags")] PostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Get current user
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                var post = new Post
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    UserID = currentUser.Id,
                    CategoryID = viewModel.CategoryID,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LikeCount = 0
                };

                _context.Add(post);
                await _context.SaveChangesAsync();

                // Add hashtags
                if (viewModel.Hashtags != null)
                {
                    foreach (var hashtagName in viewModel.Hashtags)
                    {
                        var hashtag = await _context.Hashtags
                            .FirstOrDefaultAsync(h => h.Name == hashtagName) 
                            ?? new Hashtag { Name = hashtagName };

                        if (hashtag.Id == 0)
                        {
                            _context.Hashtags.Add(hashtag);
                            await _context.SaveChangesAsync();
                        }

                        var postHashtag = new PostHashtag
                        {
                            PostId = post.Id,
                            HashtagId = hashtag.Id
                        };

                        _context.PostHashtags.Add(postHashtag);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            viewModel.AvailableCategories = await _context.Categories.ToListAsync();
            return View(viewModel);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            // Check if current user is the post owner
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (currentUser == null || post.UserID != currentUser.Id)
            {
                return Unauthorized();
            }

            var viewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                UserID = post.UserID,
                CategoryID = post.CategoryID,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikeCount = post.LikeCount,
                Hashtags = post.PostHashtags.Select(ph => ph.Hashtag.Name).ToList(),
                AvailableCategories = await _context.Categories.ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CategoryID,Hashtags")] PostViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var post = await _context.Posts
                        .Include(p => p.PostHashtags)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (post == null)
                    {
                        return NotFound();
                    }

                    // Check if current user is the post owner
                    var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
                    if (currentUser == null || post.UserID != currentUser.Id)
                    {
                        return Unauthorized();
                    }

                    // Update post properties
                    post.Title = viewModel.Title;
                    post.Description = viewModel.Description;
                    post.UpdatedAt = DateTime.UtcNow;

                    // Update hashtags
                    var currentHashtags = post.PostHashtags.Select(ph => ph.Hashtag.Name).ToList();
                    var newHashtags = viewModel.Hashtags ?? new List<string>();

                    // Remove hashtags that are no longer present
                    foreach (var postHashtag in post.PostHashtags.ToList())
                    {
                        if (!newHashtags.Contains(postHashtag.Hashtag.Name))
                        {
                            _context.PostHashtags.Remove(postHashtag);
                        }
                    }

                    // Add new hashtags
                    foreach (var hashtagName in newHashtags)
                    {
                        if (!currentHashtags.Contains(hashtagName))
                        {
                            var hashtag = await _context.Hashtags
                                .FirstOrDefaultAsync(h => h.Name == hashtagName)
                                ?? new Hashtag { Name = hashtagName };

                            if (hashtag.Id == 0)
                            {
                                _context.Hashtags.Add(hashtag);
                                await _context.SaveChangesAsync();
                            }

                            var postHashtag = new PostHashtag
                            {
                                PostId = post.Id,
                                HashtagId = hashtag.Id
                            };

                            _context.PostHashtags.Add(postHashtag);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            viewModel.AvailableCategories = await _context.Categories.ToListAsync();
            return View(viewModel);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.PostHashtags)
                    .ThenInclude(ph => ph.Hashtag)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            // Check if current user is the post owner
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (currentUser == null || post.UserID != currentUser.Id)
            {
                return Unauthorized();
            }

            var viewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                UserID = post.UserID,
                CategoryID = post.CategoryID,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikeCount = post.LikeCount,
                Username = post.User?.UserName ?? "Unknown User",
                CategoryName = post.Category?.Name ?? "Uncategorized",
                Hashtags = post.PostHashtags.Select(ph => ph.Hashtag.Name).ToList()
            };

            return View(viewModel);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts
                .Include(p => p.PostHashtags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            // Check if current user is the post owner
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (currentUser == null || post.UserID != currentUser.Id)
            {
                return Unauthorized();
            }

            // Remove post hashtags first
            _context.PostHashtags.RemoveRange(post.PostHashtags);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
