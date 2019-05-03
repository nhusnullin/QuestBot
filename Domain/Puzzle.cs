using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoreBot
{
    public class Scenario
    {
        public Scenario()
        {
            Puzzles = new Dictionary<string, Puzzle>();
            Collection = new List<Puzzle>();
        }

        [JsonIgnoreAttribute]
        public Dictionary<string, Puzzle> Puzzles { get; set; }
        public string ScenarioId { get; set; }

        public IList<Puzzle> Collection { get; set; }

        public Puzzle Add(Puzzle puzzle)
        {
            Puzzles.Add(puzzle.Id, puzzle); Collection.Add(puzzle);
            return puzzle;
        }
    }

    /// <summary>
    /// Дерево заданий квеста
    /// </summary>
    public class Puzzle
    {
        public Puzzle()
        {
            GoToYesBranch = "game over";
            GoToNoBranch = "game over";
        }

        public Puzzle(string id):this()
        {
            Id = id;
        }

        public Puzzle To(Scenario scenario)
        {
            return scenario.Add(this);

        }

        public string Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public PuzzleType PuzzleType { get; set; }
      
        /// <summary>
        /// надо дождаться пока пользователь ответит правильно на вопрос
        /// </summary>
        public bool? WaitUntilReceiveRightAnswer { get; set; }

        /// <summary>
        /// кол-во попыток на получение правильного ответа
        /// </summary>
        public int? NumberOfAttempts { get; set; }

        /// <summary>
        /// время ожидания перед тем как можно переходить на след шаг
        /// </summary>
        public int? WaitnigTime { get; set; }

        /// <summary>
        /// идентификатор пазла - ветка развития если ответил правильно
        /// </summary>
        public string GoToYesBranch { get; set; }

        /// <summary>
        /// идентификатор пазла - ветка развития если ответил НЕ правильно
        /// </summary>
        public string GoToNoBranch { get; set; }

    }
}