using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using ScenarioBot.Domain;

namespace ScenarioBot.Service
{
    public interface IScenarioService
    {
        Puzzle GetNextPuzzle(UserId teamId, string scenarioId, string lastPuzzleId, string lastAnswer);
        bool IsOver(UserId teamId, string scenarioId, string lastPuzzleId);
        void LoadAll();
        Task<IList<string>> GetNotCompletedScenarioNames(UserId userId);
        ScenarioDetails GetLastScenarioDetailsExceptGameOver(UserId userId, string scenarioId);
    }
}