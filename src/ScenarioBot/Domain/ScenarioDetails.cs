using System;
using Core.Domain;

namespace ScenarioBot.Domain
{
    // todo чет кажется лишняя сущность
    
    /// <summary>
    /// Класс для передачи данных из диалога в диалог 
    /// </summary>
    public class ScenarioDetails
    {
        public string ScenarioId { get; set; }
        public UserId UserId { get; set; }
        public PuzzleDetails LastPuzzleDetails { get; set; } 
    }
}