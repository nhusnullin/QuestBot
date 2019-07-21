using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;

namespace ScenarioBot.Repository.Impl.InMemory
{
    public class UserRepository : IUserRepository
    {

        public UserRepository()
        {
        }

        public async Task<User> GetUserByIdAsync(UserId userId)
        {
            throw new NotSupportedException();
        }

        public async Task InsertOrUpdateAsync(User user)
        {
            throw new NotSupportedException();
        }

        public Task DeleteUsers()
        {
            throw new NotSupportedException();
        }

        public Task<ICollection<User>> GetTeamUsersAsync(string teamId)
        {
            throw new NotSupportedException();
        }

        public Task<ICollection<User>> GetAllUsersAsync()
        {
            throw new NotSupportedException();
        }
    }
}