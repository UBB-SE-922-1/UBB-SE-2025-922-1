using System;
using DuolingoClassLibrary.Entities;
using System.Threading.Tasks;

namespace Duo.Services.Interfaces
{
    public interface IUserService
    {
        public Task SetUserAsync(string username);

        public Task<User> GetCurrentUserAsync();

        public Task<User> GetUserByIdAsync(int id);

        public Task<User> GetUserByUsernameAsync(string username);
    }
} 