using System.Threading.Tasks;

namespace CoreBot
{
    public interface IUserService
    {
        void SetAnswer(string userOrTeamId, string scenarioId, string puzzleId, string actualAnswer);

        Task<User> GetByAsync(string channelId, string userId);
        Task InsertOrMergeAsync(User user);
        void Remove(string channelId, string userId);
    }
}