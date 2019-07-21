using System;

namespace ScenarioBot.Domain
{
    /// <summary>
    /// Класс для передачи данных из диалога в диалог 
    /// </summary>
    public class ScenarioDetails
    {
        public string ScenarioId { get; set; }
        public string UserId { get; set; }

        [Obsolete("над использовать LastPuzzle", false)]
        public PuzzleDetails LastPuzzleDetails { get; set; }
        
        public Puzzle LastPuzzle { get; set; }
        
    }
}