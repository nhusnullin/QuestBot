using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Domain;
using Newtonsoft.Json;

namespace CoreBot.Service
{
    public class ReportService : IReportService
    {
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        public ReportService(IUserService userService, ITeamService teamService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
        }

        public async Task<ICollection<TeamQuestResult>> GetTeamQuestResults()
        {
            var answers = await _userService.GetAnswers();
            var scenarioDetails = answers.Select(i => JsonConvert.DeserializeObject<ScenarioDetails>(i.ScenarioDetails));
            var scenarioCompletePuzzle = scenarioDetails.Where(i => i.LastPuzzleDetails.IsRight).GroupBy(i => i.TeamId, i => i);
            var result = new List<TeamQuestResult>();
            foreach(var item in scenarioCompletePuzzle)
            {
                var teamId = item.Key;
                var scenarioScores = item.GroupBy(i => i.ScenarioId).ToDictionary(i => i.Key, i => i.Sum(j => (decimal)1));
                var resultItem = new TeamQuestResult(teamId, scenarioScores);
                

            }
            return new List<TeamQuestResult>();
        }
    }
}