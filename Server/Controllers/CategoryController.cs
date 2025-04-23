using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Entities;
using Server.Repositories.Repos;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryController(CategoryRepository _categoryRepository)
        {
            this._categoryRepository = _categoryRepository;
        }

        [HttpGet(Name = "GetAllCategories")]
        public async Task<IEnumerable<Category>> Get()
        {
            return await _categoryRepository.GetCategoriesAsync();
        }
    }
}
