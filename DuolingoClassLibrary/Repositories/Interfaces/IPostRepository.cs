using DuolingoClassLibrary.Entities;

namespace DuolingoClassLibrary.Repositories.Interfaces
{
    public interface IPostRepository
    {
        public Task<List<Post>> GetPosts();
    }
}
