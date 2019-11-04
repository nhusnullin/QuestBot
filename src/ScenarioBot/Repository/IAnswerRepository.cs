using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using ScenarioBot.Domain;

namespace ScenarioBot.Repository
{
    public interface IAnswerRepository
    {
        Task<IEnumerable<string>> GetCompletedScenarioIds(UserId userId);

        Task AddAnswer(Answer answer);
        Task<Answer> GetLastAddedAnswerFromNotCompletedScenario(UserId userId, string scenarioId);
        dynamic CalcAnswerWeights(int take);
        
        /// <summary>
        /// Закончен ли сценарий пользователем
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="scenarioId">идентификатор сценария</param>
        /// <returns></returns>
        Task<bool> IsScenarioCompletedByAsync(UserId userId, string scenarioId);

        /// <summary>
        /// Удалить все ответы из хранилища
        /// </summary>
        void RemoveBy(UserId userId);
    }
}