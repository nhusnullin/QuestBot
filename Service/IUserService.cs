namespace CoreBot
{
    public interface IUserService
    {
        void SetAnswer(string userOrTeamId, string scenarioId, string puzzleId, string actualAnswer);

        User GetBy(string channelId, string userId);
        void InsertOrMerge(User user);
        void Remove(string channelId, string userId);
    }
}