namespace CoreBot
{
    public interface IScenarioService
    {
        Puzzle GetNextPuzzle(string teamId, string scenarioId, string lastPuzzleId = "");
        bool IsOver(string teamId, string scenarioId);
    }
}