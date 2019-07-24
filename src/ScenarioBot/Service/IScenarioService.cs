using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using CoreBot;
using ScenarioBot.Domain;

namespace ScenarioBot.Service
{
    public interface IScenarioService
    {
        Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId, string lastAnswer);
        bool IsOver(string teamId, string scenarioId, string lastPuzzleId);
        void LoadAll();
        Task<IList<string>> GetNotCompletedScenarioNames(UserId userId);
    }
}