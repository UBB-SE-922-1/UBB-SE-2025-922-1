using Server.Entities;

namespace Server.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetCategoriesAsync();
    }
}
