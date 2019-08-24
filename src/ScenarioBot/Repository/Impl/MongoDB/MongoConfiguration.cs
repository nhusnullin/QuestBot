using Core.Domain;
using MongoDB.Driver;
using ScenarioBot.Domain;

namespace ScenarioBot.Repository.Impl.MongoDB
{
    public class MongoConfiguration
    {
        public MongoConfiguration(IMongoClient client)
        {
            Client = client;
            Database = Client.GetDatabase("questdb");
            Users = Database.GetCollection<User>("Users");
            Answers = Database.GetCollection<Answer>("Answers");
        }

        protected IMongoClient Client { get; }
        protected IMongoDatabase Database { get; }
        protected IMongoCollection<User> Users { get; }
        protected IMongoCollection<Answer> Answers { get; }
    }
}