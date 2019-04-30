using System.Collections.Generic;

namespace CoreBot
{
    /// <summary>
    /// Дерево заданий квеста
    /// </summary>
    public class Puzzle
    {
        public string ScenarioId { get; set; }
        public string PuzzleId { get; set; }
        public string Question { get; set; }
        public string ExpectedAnswer { get; set; }
        public PuzzleType PuzzleType { get; set; }
        
        public IList<Puzzle> Children { get; set; }

        /// <summary>
        /// надо ли ждать пока пользователь ответит таки правильно на вопрос
        /// </summary>
        public bool WaitUntilReceiveRightAnswer { get; set; }
    }
}