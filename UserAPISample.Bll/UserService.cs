using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserAPISample.Dal.Interfaces;
using UserAPISample.Model;

namespace UserAPISample.Bll
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;

        /// <summary>
        /// User service which can be used for CRUD operations.
        /// </summary>
        /// <param name="userRepository"></param>
        public UserService(IUserRepository userRepository)
        {
            _users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public Task<IEnumerable<User>> GetAsync() =>
            _users.GetAsync();

        public Task<User> GetAsync(int id) =>
            _users.GetAsync(id);

        public Task<User> CreateAsync(User user) =>
            _users.InsertAsync(user);

        public Task UpdateAsync(int id, User userIn) =>
            _users.UpdateAsync(id, userIn);

        public Task RemoveAsync(User userIn) =>
            _users.RemoveAsync(userIn);

        public Task RemoveAsync(int id) =>
            _users.RemoveAsync(id);
    }
}
