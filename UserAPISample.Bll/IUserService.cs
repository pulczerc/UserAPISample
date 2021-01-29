using System.Collections.Generic;
using System.Threading.Tasks;
using UserAPISample.Model;

namespace UserAPISample.Bll
{
    public interface IUserService
    {
        Task<User> CreateAsync(User User);
        Task<IEnumerable<User>> GetAsync();
        Task<User> GetAsync(int id);
        Task RemoveAsync(int id);
        Task RemoveAsync(User userIn);
        Task UpdateAsync(int id, User userIn);
    }
}