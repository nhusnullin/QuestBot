using System;
using System.Linq;
using CoreBot.Repositories;
using CoreBot.Service;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CoreBot.Domain;
using System.Collections.Generic;

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

        public  ScenarioDetails GetLastScenarioDetailsExceptGameOver(string teamId)
        {
            var answers = _storage
                .GetAnswersByTeamId(teamId, x => x.IsLastAnswer != true)
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

        public bool IsScenarioIsOverByTeam(string teamId, string scenarioId)
        {
            return _storage.GetAnswersByTeamId(teamId,
                answer => string.Equals(answer.ScenarioId, scenarioId, StringComparison.CurrentCultureIgnoreCase) &&
                          answer.IsLastAnswer).Any();
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

        public async Task SetAnswer(ScenarioDetails scenarioDetails)
        {
            var table = _storage.GetOrCreateTable(Answer.TableName);
            var scenarioId = scenarioDetails.ScenarioId;
            var puzzleId = scenarioDetails.LastPuzzleDetails.PuzzleId;

            var answer = new Answer(scenarioDetails.TeamId, $"{scenarioId} {puzzleId}")
            {
                ScenarioId = scenarioId,
                PuzzleId = puzzleId,
                ScenarioDetails = JsonConvert.SerializeObject(scenarioDetails),
                IsLastAnswer = scenarioDetails.LastPuzzleDetails.IsLastPuzzle
            };

            await _storage.InsertOrMergeEntityAsync(table, answer);
        }

        public async Task DeleteUsers()
        {
            await _userRepository.DeleteUsers();
            _storage.DeleteTableIfExists(Answer.TableName);
        }

        public async Task<ICollection<Answer>> GetAnswers()
        {
            var table = _storage.GetOrCreateTable(Answer.TableName);
            return await _storage.RetrieveEntitiesAsync<Answer>(table);
        }
    }
}