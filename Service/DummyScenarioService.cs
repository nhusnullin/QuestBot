using System.Collections.Generic;

namespace CoreBot
{
    public class DummyScenarioService : IScenarioService
    {
        public Dictionary<string, string> _store = new Dictionary<string, string>();
        public Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId = "")
        {
            _store[teamId] = lastPuzzleId;
            var puz1 = new Puzzle()
            {
                Question = "That dummy question still appear until system receive proper answer. So, that's the answer? (fyi, the answer is \"yes\")",
                ExpectedAnswer = "yes",
                ScenarioId = "Scenario1",
                PuzzleType = PuzzleType.TextPuzzleDialog,
                PuzzleId = "1",
                WaitUntilReceiveRightAnswer = true
            };

            var puz2 = new Puzzle()
            {
                Question = "Doesn't matter that answer is, the user will be thrown to the next question depends on his answer",
                ExpectedAnswer = "yes",
                ScenarioId = "Scenario1",
                PuzzleType = PuzzleType.TextPuzzleDialog,
                PuzzleId = "2",
                WaitUntilReceiveRightAnswer = false
            };

            var puz3 = new Puzzle()
            {
                Question = "bla-bla-bla question?",
                ExpectedAnswer = "yes",
                ScenarioId = "Scenario1",
                PuzzleType = PuzzleType.TextPuzzleDialog,
                PuzzleId = "3",
                WaitUntilReceiveRightAnswer = false
            };

            if (string.IsNullOrEmpty(lastPuzzleId))
            {
                return puz1;
            }
            else if (lastPuzzleId == "1")
            {
                return puz2;
            }
            else
            {
                return puz3;
            }
        }

        public bool IsOver(string teamId, string scenarioId)
        {
            return _store[teamId] == "2";
        }
    }
}