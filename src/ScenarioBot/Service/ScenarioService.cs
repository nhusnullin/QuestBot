using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScenarioBot.Domain;
using ScenarioBot.Repository;

namespace ScenarioBot.Service
{
    public class ScenarioService : IScenarioService
    {
        private readonly ILogger<ScenarioService> _logger;
        private readonly IAnswerRepository _answerRepository;
        private readonly IDictionary<string, Scenario> _store;

        public ScenarioService(ILogger<ScenarioService> logger,
            IAnswerRepository answerRepository)
        {
            _logger = logger;
            _answerRepository = answerRepository;
            _store = new Dictionary<string, Scenario>(StringComparer.CurrentCultureIgnoreCase);
        }

        public  async Task<IList<string>> GetNotCompletedScenarioNames(UserId teamId)
        {
            var loadedScenarioNames = _store.Select(x => x.Key.ToLower()).ToArray();
            var completedScenarioNames = await _answerRepository.GetCompletedScenarioIds(teamId);
            return loadedScenarioNames.Except(completedScenarioNames).ToList();
        }

        public Puzzle GetNextPuzzle(UserId teamId, string scenarioId, string lastPuzzleId, string lastAnswer)
        {
            var scenario = _store[scenarioId];

            if (string.IsNullOrEmpty(lastPuzzleId))
            {
                return scenario.Collection.First(x => string.Equals(x.Id, Puzzle.RootId, StringComparison.CurrentCultureIgnoreCase));
            }

            var puzzle = scenario.Collection.First(x=> string.Equals(x.Id , lastPuzzleId, StringComparison.CurrentCultureIgnoreCase));
            
            var puzzleId = puzzle.GetNextPossibleBranchId(lastAnswer);

            return scenario.Collection.First(x => string.Equals(x.Id, puzzleId, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool IsOver(UserId teamId, string scenarioId, string lastPuzzleId)
        {
            var scenario = _store[scenarioId];
            var puzzle = scenario.Collection.First(x => string.Equals(x.Id , lastPuzzleId, StringComparison.CurrentCultureIgnoreCase));
            return puzzle.IsLastPuzzle;
        }

        private Scenario Load(string path)
        {
            try
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
                var value = File.ReadAllText(path);
                var scenario = JsonConvert.DeserializeObject<Scenario>(value);
                _store[scenario.ScenarioId] = scenario;
                return scenario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "While loading scenario {@path}", path);
            }

            return null;
        }

        public void LoadAll()
        {
            var dr = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "raw_data"));
            dr.GetFiles("*.json").Select(x => Load(x.FullName))
                .Where(x => x != null)
                .ToList();
        }
    }
}