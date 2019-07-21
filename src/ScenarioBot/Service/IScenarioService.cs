using System.Collections.Generic;
using CoreBot;
using ScenarioBot.Domain;

namespace ScenarioBot.Service
{
    public interface IScenarioService
    {
        Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId, string lastAnswer);
        Puzzle GetFirstPuzzle(string teamId, string scenarioId);
        bool IsOver(string teamId, string scenarioId, string lastPuzzleId);
        Scenario Load(string path);
        void LoadAll();

        IList<string> GetAvailableScenario(string userId);
        IDictionary<string, Scenario> Store { get; set; }
    }
}