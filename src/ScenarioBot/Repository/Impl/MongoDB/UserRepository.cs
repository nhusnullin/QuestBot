using System.Collections.Generic;
using System.Linq;
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
            var user = await Users.Find(x => x.UserId == userId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
            return user;
        }

        public async Task InsertOrUpdateAsync(User user)
        {
            await Users.ReplaceOneAsync(u=>u.UserId == user.UserId,
                    user, new UpdateOptions { IsUpsert = true })
                    .ConfigureAwait(false);
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