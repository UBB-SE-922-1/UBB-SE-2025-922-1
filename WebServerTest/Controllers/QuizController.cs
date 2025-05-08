using System.Threading.Tasks;
using DuolingoClassLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebServerTest.Controllers
{
    public class QuizController : Controller
    {
        private readonly ILogger<QuizController> _logger;
        private readonly IQuizService _quizService;
        private readonly ICategoryService _categoryService;

        public QuizController(
            ILogger<QuizController> logger,
            IQuizService quizService,
            ICategoryService categoryService)
        {
            _logger = logger;
            _quizService = quizService;
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

            // Get completed quizzes for the user
            var quizzes = await _quizService.GetCompletedQuizzesAsync();
            
            return View(quizzes);
        }
    }
} 