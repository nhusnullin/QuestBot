using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;

namespace ScenarioBot.Repository.Impl.InMemory
{
    public class UserRepositoryInMemory : IUserRepository
    {

        private readonly IDictionary<UserId, User> _store;

        public UserRepositoryInMemory()
        {
            _store = new Dictionary<UserId, User>();
        }

        public async Task<User> GetUserByIdAsync(UserId userId)
        {
            _store.TryGetValue(userId, out var user);
            return user;
        }

        public async Task InsertOrUpdateAsync(User user)
        {
            _store[user.UserId] = user;
        }

        public async Task DeleteUsers()
        {
            _store.Clear();
        }

        public async Task<ICollection<User>> GetAllUsersAsync()
        {
            return _store.Select(x => x.Value).ToList();
        }
    }
}