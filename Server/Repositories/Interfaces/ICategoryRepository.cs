using Server.Entities;

namespace Server.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
    }
}
