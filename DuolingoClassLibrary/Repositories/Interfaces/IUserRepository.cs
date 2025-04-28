using DuolingoClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuolingoClassLibrary.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsers();
        public Task<User> GetUser(int id);

        public Task<int> CreateUser(User user);

        public Task DeleteUser(int id);

        public Task UpdateUser(User post);
    }
}
