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
    }
}
