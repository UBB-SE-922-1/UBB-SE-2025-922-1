using Server.Entities;
using Duo.Repositories;
using System;
using Duo.Interfaces;
using Duo.Repositories.Interfaces;

namespace Duo.Services
{
    /// <summary>
    /// Provides login-related functionality.
    /// </summary>
    public class LoginService : ILoginService
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public LoginService(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Authenticates a user with the provided credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if authentication is successful; otherwise, false.</returns>
        public bool AuthenticateUser(string username, string password)
        {
            return userRepository.ValidateCredentials(username, password);
        }

        /// <summary>
        /// Gets a user by their credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The user if credentials are valid; otherwise, null.</returns>
        public User GetUserByCredentials(string username, string password)
        {
            if (AuthenticateUser(username, password))
            {
                var user = userRepository.GetUserByUsername(username);
                //Console.WriteLine($"User {user.UserName} has logged in.");
                user.OnlineStatus = true;

                userRepository.UpdateUser(user);

            }
            return userRepository.GetUserByCredentials(username, password);
        }

        /// <summary>
        /// Updates a user's status to offline when logging out.
        /// </summary>
        /// <param name="user">The user to update.</param>
        public void UpdateUserStatusOnLogout(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.OnlineStatus = false;
            user.LastActivityDate = DateTime.Now;
            userRepository.UpdateUser(user);
        }
    }
}
