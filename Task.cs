using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot
{
    public class PuzzleDetails
    {
        public PuzzleDetails()
        {

        }
        public PuzzleDetails(Puzzle puzzle)
        {
            ScenarioId = puzzle.ScenarioId;
            PuzzleId = puzzle.PuzzleId;
            Question = puzzle.Question;
            ExpectedAnswer = puzzle.ExpectedAnswer;
            WaitUntilReceiveRightAnswer = puzzle.WaitUntilReceiveRightAnswer;
        }

        public string ScenarioId { get; set; }
        public string PuzzleId { get; set; }
        public string Question { get; set; }
        public string ExpectedAnswer { get; set; }
        public string ActualAnswer { get; set; }
        public bool WaitUntilReceiveRightAnswer { get; set; }

        public bool IsRight => string.Equals(ExpectedAnswer, ActualAnswer, StringComparison.CurrentCultureIgnoreCase);
    }

    public class ScenarioDetails
    {
        public string ScenarioId { get; set; }
        public string TeamId { get; set; }

        public PuzzleDetails LastPuzzleDetails { get; set; }
    }

    public class Puzzle
    {
        public string ScenarioId { get; set; }
        public string PuzzleId { get; set; }
        public string Question { get; set; }
        public string ExpectedAnswer { get; set; }
        public PuzzleType PuzzleType { get; set; }
        public IList<Puzzle> Children { get; set; }

        public bool WaitUntilReceiveRightAnswer { get; set; }
    }

    public enum PuzzleType
    {
        TextPuzzleDialog
    }

    public interface IScenarioService
    {
        Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId = "");
        bool IsOver(string teamId, string scenarioId);
    }

    public class User
    {
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public string Name { get; set; }

        public string TeamId { get; set; }

        public bool IsCaptain { get; set; }
    }

    public interface IUserService
    {
        void SetAnswer(string userOrTeamId, string scenarioId, string puzzleId, string actualAnswer);

        User GetBy(string channelId, string userId);
        void InsertOrMerge(User user);
        void Remove(string channelId, string userId);
    }

    public class DummyUserService : IUserService
    {
        public Dictionary<string, User> _store = new Dictionary<string, User>();
        public User GetBy(string channelId, string userId)
        {
            if (_store.ContainsKey(userId))
            {
                return _store[userId];
            }
            return null;
        }

        public void InsertOrMerge(User user)
        {
            _store[user.TeamId] = user;
        }

        public void Remove(string channelId, string userId)
        {
        }

        public void SetAnswer(string userOrTeamId, string scenarioId, string puzzleId, string actualAnswer)
        {
        }
    }

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
