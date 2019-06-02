namespace ScenarioBot.Domain
{
    /// <summary>
    /// Класс для передачи данных из диалога в диалог 
    /// </summary>
    public class ScenarioDetails
    {
        public string ScenarioId { get; set; }
        public string TeamId { get; set; }

        public PuzzleDetails LastPuzzleDetails { get; set; }
    }
}