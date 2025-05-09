using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Duo.Services.Interfaces;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Services.Interfaces;

namespace Duo.Services
{
    public class UserService : IUserService
    {
        private readonly IUserHelperService _userHelperService;
        private User _currentUser;
        private static readonly object _lock = new object();

        public UserService(IUserHelperService userHelperService)
        {
            _userHelperService = userHelperService ?? throw new ArgumentNullException(nameof(userHelperService));
        }

        public async Task SetUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty", nameof(username));
            }

            try 
            {
                System.Diagnostics.Debug.WriteLine($"Setting user for username: {username}");
                var existingUser = await _userHelperService.GetUserByUsername(username);
                System.Diagnostics.Debug.WriteLine($"Found existing user: {existingUser != null}");

                if (existingUser != null)
                {
                    lock (_lock)
                    {
                        _currentUser = existingUser;
                        System.Diagnostics.Debug.WriteLine($"Current user set to: {_currentUser.UserName}");
                    }
                    await Task.Delay(100); // Add a small delay to ensure the user is set
                    return;
                }

                var newUser = new User(username);
                int userId = await _userHelperService.CreateUser(newUser);
                lock (_lock)
                {
                    _currentUser = new User(userId, username);
                    System.Diagnostics.Debug.WriteLine($"Created new user: {_currentUser.UserName}");
                }
                await Task.Delay(100); // Add a small delay to ensure the user is set
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SetUser: {ex.Message}");
                var lastAttemptUser = await _userHelperService.GetUserByUsername(username);
                if (lastAttemptUser != null)
                {
                    lock (_lock)
                    {
                        _currentUser = lastAttemptUser;
                        System.Diagnostics.Debug.WriteLine($"Recovered user from last attempt: {_currentUser.UserName}");
                    }
                    await Task.Delay(100); // Add a small delay to ensure the user is set
                    return;
                }

                throw new Exception($"Failed to create or find user: {ex.Message}", ex);
            }
        }

        public User GetCurrentUser()
        {
            lock (_lock)
            {
                System.Diagnostics.Debug.WriteLine($"Getting current user: {_currentUser?.UserName ?? "null"}");
                return _currentUser;
            }
        }

        public async Task<User> GetUserById(int id)
        {
            try
            {
                return await _userHelperService.GetUserById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get user by ID: {ex.Message}", ex);
            }
        }

        public async Task<User> GetUserByUsername(string username)
        {
            try
            {
                return await _userHelperService.GetUserByUsername(username);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ClearCurrentUser()
        {
            lock (_lock)
            {
                System.Diagnostics.Debug.WriteLine("Clearing current user");
                _currentUser = null;
            }
        }
    }
}
