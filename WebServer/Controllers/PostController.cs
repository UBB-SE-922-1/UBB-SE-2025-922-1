using Microsoft.AspNetCore.Mvc;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Proxies;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly PostRepositoryProxi _categoryRepository;

        public PostsController()
        {
            _categoryRepository = new PostRepositoryProxi();
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetPosts()
        {
            var categories = await _categoryRepository.GetPosts();
            return Ok(categories);
        }
    }
} 