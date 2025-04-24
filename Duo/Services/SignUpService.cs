using Duo.Interfaces;
using DuolingoClassLibrary.Entities;
using Duo.Repositories;
using System;
using System.Threading.Tasks;
using Duo.Repositories.Interfaces;

namespace Duo.Services
{
    public class SignUpService
    {
        private readonly IUserRepository _userRepository;

        public SignUpService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            try
            {
                var user = await Task.Run(() => _userRepository.GetUserByUsername(username));
                return user != null;
            }
            catch (Exception checkingException)
            {
                Console.WriteLine($"Error checking username: {checkingException.Message}");
                return true;
            }
        }

        public async Task<bool> RegisterUser(User user)
        {
            // Check if email exists
            if (await Task.Run(() => _userRepository.GetUserByEmail(user.Email)) != null)
                return false;

            // Check if username exists
            if (await IsUsernameTaken(user.UserName))
                return false;

            user.OnlineStatus = true;
            user.UserId = _userRepository.CreateUser(user);
            return true;
        }
    }
}
