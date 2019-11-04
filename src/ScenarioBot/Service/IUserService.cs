using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using ScenarioBot.Domain;

namespace ScenarioBot.Service
{
    public interface IUserService
    {
        Task SetAnswer(ScenarioDetails scenarioDetails);

        /// <summary>
        /// Удалить все ответы пользователя
        /// </summary>
        /// <param name="userId"></param>
        void RemoveUserAnswers(UserId userId);
        
        Task<User> GetByAsync(string channelId, string userId);
        Task InsertOrMergeAsync(User user);
        Task<IDictionary<string, int>> CalcUserWeightsAsync(int userCount);
    }
}