using System.Threading.Tasks;
using DuolingoClassLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebServerTest.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseService _courseService;
        private readonly ICategoryService _categoryService;

        public CourseController(
            ILogger<CourseController> logger,
            ICourseService courseService,
            ICategoryService categoryService)
        {
            _logger = logger;
            _courseService = courseService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            // Check if user is authenticated by checking session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                // Redirect to login if not authenticated
                return RedirectToAction("Login", "Account");
            }

            // Get categories for the Community dropdown
            ViewBag.Categories = await _categoryService.GetAllCategories();

            // Get enrolled courses for the user
            var courses = await _courseService.GetEnrolledCoursesAsync();
            
            return View(courses);
        }
    }
} 