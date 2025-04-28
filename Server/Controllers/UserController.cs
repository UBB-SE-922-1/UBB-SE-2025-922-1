using Microsoft.AspNetCore.Mvc;
using DuolingoClassLibrary.Data;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Repos;
using DuolingoClassLibrary.Repositories.Interfaces;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet(Name = "GetAllUsers")]
        public async Task<IEnumerable<User>> Get()
        {
            return await _userRepository.GetUsers();
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<User> Get(int id)
        {
            var user = await _userRepository.GetUser(id);
            return user;
        }

        [HttpPost(Name = "CreateUser")]
        public async Task<int> Create([FromBody] User user)
        {
            var userId = await _userRepository.CreateUser(user);
            return userId;
        }

        [HttpPut("{id}", Name = "UpdateUser")]
        public async Task Update(int id, [FromBody] User user)
        {
            await _userRepository.UpdateUser(user);
        }

        [HttpDelete("{id}", Name = "DeleteUser")]
        public async Task Delete(int id)
        {
            await _userRepository.DeleteUser(id);
        }
    }
} 