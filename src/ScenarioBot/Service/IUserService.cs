using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using CoreBot;
using ScenarioBot.Domain;

namespace ScenarioBot.Service
{
    public interface IUserService
    {
        Task SetAnswer(ScenarioDetails scenarioDetails);
        
        Task<User> GetByAsync(string channelId, string userId);
        Task InsertOrMergeAsync(User user);
        bool IsScenarioIsOverByTeam(string teamId, string scenarioId);
        IDictionary<string, int> CalcUserWeights(IDictionary<string, Scenario> scenarioStore);
    }
}