using CoreBot.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBot.Repositories
{
    public interface IUserRepository
    {
        Task<User> TryGetUserByIdAsync(UserId userId);
        Task InsertOrMergeAsync(User user);
    }
}
