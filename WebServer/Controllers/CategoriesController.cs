using Microsoft.AspNetCore.Mvc;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Proxies;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryRepositoryProxi _categoryRepository;

        public CategoriesController()
        {
            _categoryRepository = new CategoryRepositoryProxi();
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _categoryRepository.GetCategoriesAsync();
            return Ok(categories);
        }
    }
} 