using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoreBot
{
    public class Scenario
    {
        public Scenario()
        {
            Collection = new List<Puzzle>();
        }

        public string ScenarioId { get; set; }

        public IList<Puzzle> Collection { get; set; }

        public Puzzle Add(Puzzle puzzle)
        {
            Collection.Add(puzzle);
            return puzzle;
        }
    }

    /// <summary>
    /// Дерево заданий квеста
    /// </summary>
    public class Puzzle
    {
        private const string _gameOverid = "Game over";
        public const string RootId = "Root";

        public Puzzle()
        {
            GoToYesBranch = _gameOverid;
            GoToNoBranch = _gameOverid;
        }

        public Puzzle(string id):this()
        {
            Id = id;
        }

        public bool IsLastPuzzle => string.Equals(Id, _gameOverid, StringComparison.CurrentCultureIgnoreCase);
        
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