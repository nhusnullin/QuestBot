using CoreBot.Repositories;
using CoreBot.Service;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CoreBot
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICloudStorage _storage;
        public UserService(IUserRepository userRepository, ICloudStorage cloudStorage)
        {
            _userRepository = userRepository ?? throw new System.ArgumentNullException(nameof(userRepository));
            _storage = cloudStorage ?? throw new System.ArgumentNullException(nameof(cloudStorage));
        }

        public async Task<User> GetByAsync(string channelId, string userId)
        {
            return await _userRepository.TryGetUserByIdAsync(new Domain.UserId(channelId, userId));
        }

        public async Task InsertOrMergeAsync(User user)
        {
            await _userRepository.InsertOrMergeAsync(user);
        }

        public void Remove(string channelId, string userId)
        {
        }

        public async Task SetAnswer(string channelId, string userId, string scenarioId, string puzzleId, ScenarioDetails scenarioDetails)
        {
            var table = _storage.GetOrCreateTable(Answer.TableName);

            var answer = new Answer(userId, $"{scenarioId} {puzzleId}")
            {
                ScenarioId = scenarioId,
                PuzzleId = puzzleId,
                ScenarioDetails = JsonConvert.SerializeObject(scenarioDetails)
            };

            await _storage.InsertOrMergeEntityAsync(table, answer);
        }
    }
}