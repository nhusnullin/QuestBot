using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;

namespace ScenarioBot.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(UserId userId);
        Task InsertOrUpdateAsync(User user);
        Task DeleteUsers();
        Task<ICollection<User>> GetAllUsersAsync();
    }
}