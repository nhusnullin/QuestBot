using CoreBot.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public interface IUserService
    {
        Task SetAnswer(ScenarioDetails scenarioDetails);
        ScenarioDetails GetLastScenarioDetailsExceptGameOver(string teamId);
        Task<User> GetByAsync(string channelId, string userId);
        Task InsertOrMergeAsync(User user);
        Task DeleteUsers();
        bool IsScenarioIsOverByTeam(string teamId, string scenarioId);
        Task<ICollection<Answer>> GetAnswers();
        
    }
}