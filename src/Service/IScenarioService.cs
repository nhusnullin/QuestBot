using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreBot.Service;
using Newtonsoft.Json;

namespace CoreBot
{
    public interface IScenarioService
    {
        Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId, string lastAnswer);
        Puzzle GetFirstPuzzle(string teamId, string scenarioId);
        bool IsOver(string teamId, string scenarioId, string lastPuzzleId);

        Scenario Load(string path);
        void LoadAll();

        string[] AvailableScenario { get; }
    }

    public class ScenarioService : IScenarioService
    {
        public Dictionary<string, Scenario> _store = new Dictionary<string, Scenario>();

        public Puzzle GetFirstPuzzle(string teamId, string scenarioId)
        {
            var scenario = _store[scenarioId];
            return scenario.Collection.First(x => string.Equals(x.Id, Puzzle.RootId, StringComparison.CurrentCultureIgnoreCase));
        }

        public string[] AvailableScenario => _store.Select(x => x.Key).ToArray();

        public Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId, string lastAnswer)
        {
            var scenario = _store[scenarioId];

            if (string.IsNullOrEmpty(lastPuzzleId))
            {
                return GetFirstPuzzle(teamId, scenarioId);
            }

            var puzzle = scenario.Collection.First(x=> string.Equals(x.Id , lastPuzzleId, StringComparison.CurrentCultureIgnoreCase));
            var puzzleId = puzzle.GetNextPossibleBranchId(lastAnswer);

            return scenario.Collection.First(x => string.Equals(x.Id, puzzleId, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool IsOver(string teamId, string scenarioId, string lastPuzzleId)
        {
            var scenario = _store[scenarioId];
            var puzzle = scenario.Collection.First(x => string.Equals(x.Id , lastPuzzleId, StringComparison.CurrentCultureIgnoreCase));
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

        public void LoadAll()
        {
            var dr = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "raw_data"));
            dr.GetFiles("*.json").Select(x => Load(x.FullName)).ToList();
        }
    }
}