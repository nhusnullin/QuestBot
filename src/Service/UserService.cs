using System.Linq;
using CoreBot.Service;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CoreBot
{
    public class UserService : IUserService
    {
        private readonly ICloudStorage _storage;

        public UserService(ICloudStorage storage)
        {
            _storage = storage;
        }

        public  ScenarioDetails GetLastScenarioDetailsExceptGameOver(string channelId, string userId)
        {
            var answers = _storage
                .GetAnswersByUserId(userId, x => x.IsLastAnswer != true)
                .OrderByDescending(x => x.Timestamp)
                .Take(1)
                .ToList();

            var scenarioDetails = answers.FirstOrDefault()?.ScenarioDetails;

            if (string.IsNullOrEmpty(scenarioDetails))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ScenarioDetails>(scenarioDetails);
        }

        public async Task<User> GetByAsync(string channelId, string userId)
        {
            var table = _storage.GetOrCreateTable(User.TableName);
            return await _storage.RetrieveEntityByAsync<User>(table, channelId, userId);
        }

        public async Task InsertOrMergeAsync(User user)
        {
            var table = _storage.GetOrCreateTable(User.TableName);
            await _storage.InsertOrMergeEntityAsync(table, user);
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
                ScenarioDetails = JsonConvert.SerializeObject(scenarioDetails),
                IsLastAnswer = scenarioDetails.LastPuzzleDetails?.IsLastPuzzle ?? false
            };

            await _storage.InsertOrMergeEntityAsync(table, answer);
        }
    }
}