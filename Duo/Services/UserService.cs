using System;
using System.Collections.Generic;
using DuolingoClassLibrary.Entities;
using Duo.Repositories.Interfaces;
using Duo.Services.Interfaces;
using System.Threading.Tasks;

namespace Duo.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private User _currentUser;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task SetUserAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty", nameof(username));
            }

            try 
            {
                var existingUser = await GetUserByUsernameAsync(username);

                if (existingUser != null)
                {
                    _currentUser = existingUser;
                    return;
                }

                var newUser = new User();
                int userId = await Task.Run(() => _userRepository.CreateUser(newUser));
                _currentUser = new User(userId, username);
            }
            catch (Exception ex)
            {
                var lastAttemptUser = await GetUserByUsernameAsync(username);
                if (lastAttemptUser != null)
                {
                    _currentUser = lastAttemptUser;
                    return;
                }

                throw new Exception($"Failed to create or find user: {ex.Message}", ex);
            }
        }

        public async Task<User> GetCurrentUserAsync()
        {
            if (_currentUser == null)
            {
                throw new InvalidOperationException("No user is currently logged in.");
            }
            return await Task.FromResult(_currentUser);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                return await Task.Run(() => _userRepository.GetUserById(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get user by ID: {ex.Message}", ex);
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await Task.Run(() => _userRepository.GetUserByUsername(username));
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        // Keep the non-async methods for backward compatibility
        
        public void setUser(string username)
        {
            SetUserAsync(username).GetAwaiter().GetResult();
        }

        public User GetCurrentUser()
        {
            return GetCurrentUserAsync().GetAwaiter().GetResult();
        }

        public User GetUserById(int id)
        {
            return GetUserByIdAsync(id).GetAwaiter().GetResult();
        }

        public User GetUserByUsername(string username)
        {
            return GetUserByUsernameAsync(username).GetAwaiter().GetResult();
        }
    }
}
