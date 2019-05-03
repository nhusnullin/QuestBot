using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreBot.Service;
using Newtonsoft.Json;

namespace CoreBot
{
    public interface IScenarioService
    {
        Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId = "", bool? lastWasRight = false);
        Puzzle GetFirstPuzzle(string teamId, string scenarioId);
        bool IsOver(string teamId, string scenarioId, string lastPuzzleId);

        Scenario Load(string path);
    }

    public class ScenarioService : IScenarioService
    {
        public Dictionary<string, Scenario> _store = new Dictionary<string, Scenario>();

        public Puzzle GetFirstPuzzle(string teamId, string scenarioId)
        {
            var scenario = _store[scenarioId];
            return scenario.Collection.First(x => x.Id == Puzzle.RootId);
        }

        public Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId = "", bool? lastWasRight = false)
        {
            var scenario = _store[scenarioId];

            if (string.IsNullOrEmpty(lastPuzzleId))
            {
                return GetFirstPuzzle(teamId, scenarioId);
            }

            Puzzle puzzle = scenario.Collection.First(x=>x.Id == lastPuzzleId);
            if (lastWasRight.HasValue && lastWasRight.Value)
            {
                return scenario.Collection.First(x => x.Id == puzzle.GoToYesBranch); 
            }
            return scenario.Collection.First(x => x.Id == puzzle.GoToNoBranch);
        }

        public bool IsOver(string teamId, string scenarioId, string lastPuzzleId)
        {
            var scenario = _store[scenarioId];
            var puzzle = scenario.Collection.First(x => x.Id == lastPuzzleId);
            return puzzle.IsLastPuzzle;
        }

        public Scenario Load(string path)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            var value = File.ReadAllText(path);
            var scenario = JsonConvert.DeserializeObject<Scenario>(value);
            _store[scenario.ScenarioId] = scenario;
            return scenario;
        }
    }
}