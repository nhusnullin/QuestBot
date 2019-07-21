using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using CoreBot;
using Microsoft.Bot.Builder;
using ScenarioBot.Domain;
using ScenarioBot.Repository;

namespace ScenarioBot.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new System.ArgumentNullException(nameof(userRepository));
        }

        public  ScenarioDetails GetLastScenarioDetailsExceptGameOver(string userId)
        {
                return new ScenarioDetails()
                {
                    ScenarioId = "testScenario",
                    UserId = userId
                };
                
            throw new NotImplementedException();
//            var answers = _storage
//                .GetAnswersByTeamId(teamId, x => x.IsLastAnswer != true)
//                .OrderByDescending(x => x.Timestamp)
//                .Take(1)
//                .ToList();
//
//            var scenarioDetails = answers.FirstOrDefault()?.ScenarioDetails;
//
//            if (string.IsNullOrEmpty(scenarioDetails))
//            {
//                return null;
//            }
//
//            return JsonConvert.DeserializeObject<ScenarioDetails>(scenarioDetails);
        }

        public bool IsScenarioIsOverByTeam(string teamId, string scenarioId)
        {
            return false;
//            return _storage.GetAnswersByTeamId(teamId,
//                answer => string.Equals(answer.ScenarioId, scenarioId, StringComparison.CurrentCultureIgnoreCase) &&
//                          answer.IsLastAnswer).Any();
        }

        public async Task<User> GetByAsync(string channelId, string userId)
        {
            return await _userRepository.GetUserByIdAsync(new UserId(channelId, userId));
        }

        public async Task InsertOrMergeAsync(User user)
        {
            await _userRepository.InsertOrUpdateAsync(user);
        }

        public void Remove(string channelId, string userId)
        {
        }

        public async Task SetAnswer(ScenarioDetails scenarioDetails)
        {
            throw new NotImplementedException();
            
//            var table = _storage.GetOrCreateTable(Answer.TableName);
//            var scenarioId = scenarioDetails.ScenarioId;
//            var puzzleId = scenarioDetails.LastPuzzleDetails.PuzzleId;
//
//            var answer = new Answer(scenarioDetails.TeamId, $"{scenarioId} {puzzleId}")
//            {
//                ScenarioId = scenarioId,
//                PuzzleId = puzzleId,
//                ScenarioDetails = JsonConvert.SerializeObject(scenarioDetails),
//                IsLastAnswer = scenarioDetails.LastPuzzleDetails.IsLastPuzzle
//            };
//
//            await _storage.InsertOrMergeEntityAsync(table, answer);
        }

        public async Task DeleteUsers()
        {
//            await _userRepository.DeleteUsers();
//            _storage.DeleteTableIfExists(Answer.TableName);
        }

        public async Task<ICollection<Answer>> GetAnswers()
        {
            throw  new NotImplementedException();
//            var table = _storage.GetOrCreateTable(Answer.TableName);
//            return await _storage.RetrieveEntitiesAsync<Answer>(table);
        }

        public IDictionary<string, int> CalcUserWeights(IDictionary<string, Scenario> scenarioStore)
        {
            var result = new Dictionary<string, int>();

//            var allAnswers = _storage.GetAllAnswers();
//
//            foreach (var answer in allAnswers)
//            {
//                if (!result.ContainsKey(answer.PartitionKey))
//                {
//                    result[answer.PartitionKey] = 0;
//                }
//
//                scenarioStore.TryGetValue(answer.ScenarioId, out var scenario);
//                var weight = scenario
//                    ?.Collection
//                    ?.FirstOrDefault(x => string.Equals(x.Id, answer.PuzzleId, StringComparison.CurrentCultureIgnoreCase))
//                    ?.Weight;
//                result[answer.PartitionKey] += weight ?? 0;
//            }

            return result;
        }
    }
}