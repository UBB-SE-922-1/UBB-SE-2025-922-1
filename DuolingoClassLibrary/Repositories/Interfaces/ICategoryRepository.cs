using DuolingoClassLibrary.Entities;

namespace DuolingoClassLibrary.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetCategoriesAsync();
    }
}
