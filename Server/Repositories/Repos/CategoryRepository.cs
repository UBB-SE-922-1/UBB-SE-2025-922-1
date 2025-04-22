using Server.Data;
using Server.Entities;
using Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Server.Repositories.Repos
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;

        public CategoryRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Category> GetCategories()
        {
            try
            {
                return _context.Categories.ToList();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error getting categories: {ex.Message}");
                return new List<Category>();
            }
        }
    }
}
