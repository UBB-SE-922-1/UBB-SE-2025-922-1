using DuolingoClassLibrary.Data;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DuolingoClassLibrary.Repositories.Repos
{
    public class PostRepository : IPostRepository
    {
        DataContext _context;

        public PostRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Post>> GetPosts()
        {
            try
            {
                var posts = await _context.Posts.ToListAsync();
                Console.WriteLine($"Retrieved {posts.Count} posts.");

                return await _context.Posts.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error getting categories: {ex.Message}");
                return new List<Post>();
            }
        }

        public async Task<int> CreatePost(Post post)
        {
            try
            {
                if (post == null)
                {
                    throw new ArgumentNullException(nameof(post));
                }
                _context.Posts.Add(post);
                _context.SaveChanges();
                return await Task.FromResult(post.Id);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error creating post: {ex.Message}");
                return await Task.FromResult(-1);
            }
        }
        
        public async Task DeletePost(int id)
        {
            try
            {
                var post = await _context.Posts.FindAsync(id);
                if (post != null)
                {
                    _context.Posts.Remove(post);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error deleting post: {ex.Message}");
            }
        }
        
        public async Task UpdatePost(Post post)
        {
            try
            {
                if (post == null)
                {
                    throw new ArgumentNullException(nameof(post));
                }
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error updating post: {ex.Message}");
            }
        }
    }
}
