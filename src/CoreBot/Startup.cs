using System.Collections.Generic;
using System.Configuration;
using Core.BotCommands;
using Core.Service;
using CoreBot.BotCommands;
using CoreBot.Bots;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ScenarioBot;
using ScenarioBot.BotCommands;
using ScenarioBot.Dialogs;
using ScenarioBot.Repository;
using ScenarioBot.Repository.Impl.InMemory;
using ScenarioBot.Repository.Impl.MongoDB;
using ScenarioBot.Service;

namespace CoreBot
{
    public class Startup
    {
        private const string BotOpenIdMetadataKey = "BotOpenIdMetadata";

        public Startup(IHostingEnvironment env)
        {
            var _isProduction = env.IsProduction();
            Configuration = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                    .AddEnvironmentVariables()
                    .Build()
                ;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            if (!string.IsNullOrEmpty(Configuration[BotOpenIdMetadataKey]))
                ChannelValidation.OpenIdMetadataUrl = Configuration[BotOpenIdMetadataKey];

            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the channel provider to be used with the Bot Framework Adapter.
            services.AddSingleton<IChannelProvider, ConfigurationChannelProvider>();

            // Create the Bot Framework Adapter with error handling enabled. 
            services.AddSingleton<IBotFrameworkHttpAdapter, BotHttpAdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.) 
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            var inMemoryRepositories = Configuration.GetSection("InMemoryRepositories").Value == "true";
            if (inMemoryRepositories)
            {
                services.AddSingleton<IAnswerRepository, AnswerRepositoryInMemory>();
                services.AddSingleton<IUserRepository, UserRepositoryInMemory>();
            }
            else
            {
                services.AddSingleton<IAnswerRepository, AnswerRepository>();
                services.AddSingleton<IUserRepository, UserRepository>();
                services.AddSingleton<IMongoClient, MongoClient>(
                    client =>
                    {
                        var connectionString = Configuration.GetConnectionString("MongoConnection");
                        return new MongoClient(connectionString);
                    });
            }

            services.AddSingleton<IScenarioService, ScenarioService>();
            services.AddSingleton<IUserService, UserService>();
            
            services.AddSingleton<HelpBotCommand, HelpBotCommand>();
            services.AddSingleton<ScenarioBotCommand, ScenarioBotCommand>();
            services.AddSingleton<TopCommand, TopCommand>();
            services.AddSingleton<ClearCommand, ClearCommand>();
            services.AddSingleton<IList<IBotCommand>>(x => new List<IBotCommand>
            {
                x.GetRequiredService<HelpBotCommand>(),
                x.GetRequiredService<ScenarioBotCommand>(),
                x.GetRequiredService<TopCommand>(),
                x.GetRequiredService<ClearCommand>(),
            });


//            // The Dialog that will be run by the bot.
//            services.AddSingleton<MainDialog>();
//            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
//            services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();

            // The Dialog that will be run by the bot.
            services.AddSingleton<ScenarioDialog>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogAndWelcomeBot<ScenarioDialog>>();

            string botAppId = Configuration.GetSection("MicrosoftAppId").Value;
            string password = Configuration.GetSection("MicrosoftAppPassword").Value;
            services.AddSingleton<IAdapterIntegration>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IAdapterIntegration>>();
                var adapter = new BotAdapterWithErrorHandler(new SimpleCredentialProvider(botAppId, password),logger);
                return adapter;
            });

            services.AddSingleton<INotificationService>(sp => new NotificationService(botAppId, sp.GetRequiredService<IAdapterIntegration>()));

            services.AddHostedService<LoadScenarioService>();
            //services.AddHostedService<SendNotifyInBackgroundService>();


            services.AddHealthChecks();
        }

//        public static async Task<IEnumerable<KeyValuePair<UserId, ConversationReference>>> LoadConversationReferences(ICloudStorage cloudStorage)
//        {

//            throw new NotImplementedException();
//            var table = cloudStorage.GetOrCreateTable(User.TableName);
//            var users = await cloudStorage.RetrieveEntitiesAsync<User>(table);
//            return users.Where(i => i.ConversationData != null).Select(i => new KeyValuePair<UserId, ConversationReference>(
//                new UserId(i.PartitionKey, i.RowKey), JsonConvert.DeserializeObject<ConversationReference>(i.ConversationData)));
//        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app
                .UseMvc()
                .UseHealthChecks("/hc");
        }
    }
}