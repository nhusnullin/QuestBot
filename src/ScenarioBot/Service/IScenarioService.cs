using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using ScenarioBot.Domain;

namespace ScenarioBot.Service
{
    public interface IScenarioService
    {
        Puzzle GetNextPuzzle(UserId teamId, string scenarioId, string lastPuzzleId, string lastAnswer);
        bool IsOver(string scenarioId, string lastPuzzleId);
        
        /// <summary>
        /// Прохождение сценария пользователем окончено   
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="scenarioId">идентификатор сценария</param>
        /// <returns>закончил ли пользователь прохождение сценария</returns>
        Task<bool> IsOverByUserAsync(UserId userId, string scenarioId);
        
        void LoadAll();
        Task<IList<string>> GetNotCompletedScenarioNames(UserId userId);
        Task<ScenarioDetails> GetLastScenarioDetailsExceptGameOver(UserId userId, string scenarioId);
        bool IsExist(string scenarioId);
    }
}