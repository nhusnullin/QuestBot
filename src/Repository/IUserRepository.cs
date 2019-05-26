using System.Collections.Generic;
using System.Threading.Tasks;
using CoreBot.Domain;

namespace CoreBot.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(UserId userId);
        Task InsertOrUpdateAsync(User user);
        Task DeleteUsers();
        Task<ICollection<User>> GetTeamUsersAsync(string teamId);
        Task<ICollection<User>> GetAllUsersAsync();
    }
}
