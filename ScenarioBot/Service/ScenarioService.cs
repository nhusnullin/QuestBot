using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreBot;
using Newtonsoft.Json;
using ScenarioBot.Domain;

namespace ScenarioBot.Service
{
    public class ScenarioService : IScenarioService
    {
        private readonly ICloudStorage _cloudStorage;
        public IDictionary<string, Scenario> Store { get; set; }

        public ScenarioService()
        {
            Store = new Dictionary<string, Scenario>(StringComparer.CurrentCultureIgnoreCase);
        }

        public ScenarioService(ICloudStorage cloudStorage):this()
        {
            _cloudStorage = cloudStorage;
        }


        public Puzzle GetFirstPuzzle(string teamId, string scenarioId)
        {
            var scenario = Store[scenarioId];
            return scenario.Collection.First(x => string.Equals(x.Id, Puzzle.RootId, StringComparison.CurrentCultureIgnoreCase));
        }

        public IList<string> GetAvailableScenario(string teamId)
        {
            var loadedScenario = Store.Select(x => x.Key.ToLower()).ToArray();

            var completedScenario = _cloudStorage.GetAnswersByTeamId(teamId, answer => answer.IsLastAnswer)
                .GroupBy(x=>x.ScenarioId);

            return loadedScenario.Except(completedScenario.Select(x => x.Key.ToLower())).ToList();
        }

        public Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId, string lastAnswer)
        {
            var scenario = Store[scenarioId];

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
            var scenario = Store[scenarioId];
            var puzzle = scenario.Collection.First(x => string.Equals(x.Id , lastPuzzleId, StringComparison.CurrentCultureIgnoreCase));
            return puzzle.IsLastPuzzle;
        }

        public Scenario Load(string path)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            var value = File.ReadAllText(path);
            var scenario = JsonConvert.DeserializeObject<Scenario>(value);
            Store[scenario.ScenarioId] = scenario;
            return scenario;
        }

        public void LoadAll()
        {
            var dr = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "raw_data"));
            dr.GetFiles("*.json").Select(x => Load(x.FullName)).ToList();
        }
    }
}