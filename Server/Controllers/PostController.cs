using Microsoft.AspNetCore.Mvc;
using DuolingoClassLibrary.Data;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Repos;
using DuolingoClassLibrary.Repositories.Interfaces;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly PostRepository _postRepository;

        public PostController(PostRepository _postRepository)
        {
            this._postRepository = _postRepository;
        }
        [HttpGet(Name = "GetAllPosts")]
        public async Task<IEnumerable<Post>> Get()
        {
            return await _postRepository.GetPosts();
        }
    }
}

