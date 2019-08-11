using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using ScenarioBot.Domain;
using ScenarioBot.Repository;

namespace ScenarioBot.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAnswerRepository _answerRepository;

        public UserService(IUserRepository userRepository, IAnswerRepository answerRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _answerRepository = answerRepository;
        }

        public  ScenarioDetails GetLastScenarioDetailsExceptGameOver(UserId userId)
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
            await _answerRepository.AddAnswer(new Answer(scenarioDetails));
        }

        public async Task DeleteUsers()
        {
        }

        public async Task<ICollection<Answer>> GetAnswers()
        {
            return new List<Answer>(); 
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