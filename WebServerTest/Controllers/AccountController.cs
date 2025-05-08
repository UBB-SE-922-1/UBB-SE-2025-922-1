using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Services;
using DuolingoClassLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebServerTest.Models;

namespace WebServerTest.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IUserHelperService _userHelperService;
        private readonly SignUpService _signUpService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ILoginService loginService, 
            IUserHelperService userHelperService,
            SignUpService signUpService,
            ILogger<AccountController> logger)
        {
            _loginService = loginService;
            _userHelperService = userHelperService;
            _signUpService = signUpService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Check if user is already logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            _logger.LogInformation($"Login GET - User is {(userId.HasValue ? "already logged in" : "not logged in")}");
            
            if (userId.HasValue)
            {
                _logger.LogInformation($"Login GET - Redirecting already logged in user (ID: {userId}) to Home/Index");
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl ?? string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation($"Login POST - Attempt for username: {model.Username}");
            
            // Set a default empty ReturnUrl if it's null
            model.ReturnUrl ??= string.Empty;
            
            // Remove ReturnUrl validation errors if any
            if (ModelState.ContainsKey("ReturnUrl"))
            {
                ModelState.Remove("ReturnUrl");
            }
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login POST - Model state is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                }
                return View(model);
            }

            try
            {
                _logger.LogInformation($"Login POST - Attempting to authenticate user: {model.Username}");
                var isAuthenticated = await _loginService.AuthenticateUser(model.Username, model.Password);
                _logger.LogInformation($"Login POST - Authentication result for {model.Username}: {(isAuthenticated ? "Success" : "Failed")}");
                
                if (isAuthenticated)
                {
                    // Get the user
                    _logger.LogInformation($"Login POST - Getting user details for {model.Username}");
                    var user = await _loginService.GetUserByCredentials(model.Username, model.Password);
                    
                    if (user != null)
                    {
                        _logger.LogInformation($"Login POST - User found. ID: {user.UserId}, Username: {user.UserName}");
                        
                        // Store user ID in session or cookie
                        HttpContext.Session.SetInt32("UserId", user.UserId);
                        HttpContext.Session.SetString("Username", user.UserName);
                        
                        _logger.LogInformation($"Login POST - Session values set. UserId: {user.UserId}, Username: {user.UserName}");
                        
                        // Debug info
                        Debug.WriteLine($"Login successful. Username: {user.UserName}, UserID: {user.UserId}");

                        // Redirect to home page or return URL
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            _logger.LogInformation($"Login POST - Redirecting to return URL: {model.ReturnUrl}");
                            Debug.WriteLine($"Redirecting to return URL: {model.ReturnUrl}");
                            return Redirect(model.ReturnUrl);
                        }
                        
                        _logger.LogInformation("Login POST - Redirecting to Home/Index");
                        Debug.WriteLine("Redirecting to Home/Index");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _logger.LogWarning("Login POST - User is null after authentication");
                        Debug.WriteLine("User is null after authentication");
                        ModelState.AddModelError(string.Empty, "User account could not be retrieved.");
                    }
                }
                else
                {
                    _logger.LogWarning($"Login POST - Authentication failed for username: {model.Username}");
                    Debug.WriteLine("Authentication failed");
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login POST - Exception: {ex.Message}");
                Debug.WriteLine($"Login error: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Error during login: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel { ShowPassword = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check if username is already taken
                if (await _signUpService.IsUsernameTaken(model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken");
                    return View(model);
                }

                // Create the user entity
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    DateJoined = DateTime.Now,
                    OnlineStatus = true
                };

                // Register the user
                var isRegistered = await _signUpService.RegisterUser(user);
                if (isRegistered)
                {
                    // Store user ID in session
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("Username", user.UserName);

                    Debug.WriteLine($"Registration successful. Username: {user.UserName}, UserID: {user.UserId}");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Error registering user. Email may already be in use.");
                return View(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Registration error: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Error during registration: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    var user = await _userHelperService.GetUserById(userId.Value);
                    if (user != null)
                    {
                        await _loginService.UpdateUserStatusOnLogout(user);
                    }
                }

                // Clear session
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logout error: {ex.Message}");
                // Log error but continue with logout
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
} 