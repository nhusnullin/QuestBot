using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using MongoDB.Driver;

namespace ScenarioBot.Repository.Impl.MongoDB
{
    public class UserRepository : MongoConfiguration, IUserRepository
    {
        public UserRepository(IMongoClient client) : base(client)
        {
        }

        public async Task<User> GetUserByIdAsync(UserId userId)
        {
            if (userId == null)
            {
                return null;
            }
            
            var user = await Users.Find(x => x.UserId.Id == userId.Id )
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
            return user;
        }

        public async Task InsertOrUpdateAsync(User user)
        {
            // todo upsert
            if (await GetUserByIdAsync(user.UserId).ConfigureAwait(false) != null)
                await Users.ReplaceOneAsync(u => u.UserId.Id == user.UserId.Id,
                    user).ConfigureAwait(false);
            else
                await Users.InsertOneAsync(user).ConfigureAwait(false);
        }

        public async Task DeleteUsers()
        {
            await Users.DeleteManyAsync(u => true);
        }

        public async Task<ICollection<User>> GetAllUsersAsync()
        {
            return await Users.AsQueryable().ToListAsync().ConfigureAwait(false);
        }
    }
}