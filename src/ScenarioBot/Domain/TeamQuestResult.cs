using System;
using System.Collections.Generic;

namespace ScenarioBot.Domain
{
    public class TeamQuestResult
    {
        public TeamQuestResult(string teamName, IDictionary<string, decimal> scenarioScores)
        {
            TeamName = teamName ?? throw new ArgumentNullException(nameof(teamName));
            ScenarioScores = scenarioScores ?? throw new ArgumentNullException(nameof(scenarioScores));
        }

        public string TeamName { get; }
        public IDictionary<string, decimal> ScenarioScores { get; }
    }
}