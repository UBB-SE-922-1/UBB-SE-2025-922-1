using Duo.Interfaces;
using Server.Entities;
using System;

namespace Duo.ViewModels
{
    /// <summary>
    /// ViewModel that handles the login logic.
    /// </summary>
    public class LoginViewModel
    {
        private readonly ILoginService loginService;

        /// <summary>
        /// Gets or sets the username entered by the user.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the password entered by the user.
        /// </summary>
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets a value indicating whether the login was successful.
        /// </summary>
        public bool LoginStatus { get; private set; }
        
        /// <summary>
        /// Gets the logged-in user after a successful login.
        /// </summary>
        public User LoggedInUser { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="loginService">The login service.</param>
        public LoginViewModel(ILoginService loginService)
        {
            this.loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        }

        /// <summary>
        /// Attempts to log in with the provided credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public void AttemptLogin(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                LoginStatus = false;
                return;
            }

            Username = username;
            Password = password;

            // Try to get the user
            LoggedInUser = loginService.GetUserByCredentials(Username, Password);
            LoginStatus = LoggedInUser != null;
        }
    }
}
