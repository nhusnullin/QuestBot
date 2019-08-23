using Core.Domain;
using MongoDB.Driver;
using ScenarioBot.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioBot.Repository.Impl.MongoDB
{
    public class MongoConfiguration
    {
        protected IMongoClient Client { get; }
        protected IMongoDatabase Database { get; }
        protected IMongoCollection<User> Users { get; }
        protected IMongoCollection<Answer> Answers { get; }


        /// <inheritdoc />
        public MongoConfiguration(IMongoClient client)
        {
            Client = client;
           // Database = Client.GetDatabase(ConfigurationManager.AppSettings["mongoDbOrdersDatabase"]);
           // Users = Database.GetCollection<User>(ConfigurationManager.AppSettings["mongoDbOrdersCollection"]);
           // Answers = Database.GetCollection<Answer>(ConfigurationManager.AppSettings["mongoDbCountersCollection"]);
        }
    }
}
