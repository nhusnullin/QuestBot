using CoreBot.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBot.Repositories
{
    public interface IUserRepository
    {
        Task<User> TryGetUserByIdAsync(UserId userId);
        Task InsertOrMergeAsync(User user);
        Task DeleteUsers();
        Task<ICollection<User>> GetTeamUsersAsync(string teamId);
        Task<ICollection<User>> GetUsersAsync();
    }
}
