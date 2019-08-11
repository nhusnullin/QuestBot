using System;
using Core.Domain;

namespace ScenarioBot.Domain
{
    public class Answer
    {
        public Answer()
        {

        }

        public Answer(ScenarioDetails scenarioDetails)
        {
            RespondentId = scenarioDetails.UserId;
            ScenarioId = scenarioDetails.ScenarioId;
            PuzzleId = scenarioDetails.LastPuzzleDetails.PuzzleId;
            IsLastAnswer = scenarioDetails.LastPuzzleDetails.IsLastPuzzle;
            ActualAnswer = scenarioDetails.LastPuzzleDetails.ActualAnswer;
            Timestamp = DateTime.UtcNow;
        }

        public UserId RespondentId { get; }
        public string ScenarioId { get; set; }
        public string PuzzleId { get; set; }
        public bool IsLastAnswer { get; set; }
        public string ActualAnswer { get; set; }
        public DateTime Timestamp { get; set; }
    }
}