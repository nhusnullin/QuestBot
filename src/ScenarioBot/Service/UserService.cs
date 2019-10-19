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
        private readonly IAnswerRepository _answerRepository;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IAnswerRepository answerRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _answerRepository = answerRepository;
        }

        public async Task<User> GetByAsync(string channelId, string userId)
        {
            return await _userRepository.GetUserByIdAsync(new UserId(channelId, userId));
        }

        public async Task InsertOrMergeAsync(User user)
        {
            await _userRepository.InsertOrUpdateAsync(user);
        }


        public async Task SetAnswer(ScenarioDetails scenarioDetails)
        {
            await _answerRepository.AddAnswer(new Answer(scenarioDetails));
        }

        public async Task<IDictionary<string, int>> CalcUserWeightsAsync(int userCount)
        {
            var result = new Dictionary<string, int>();

            var answerWeights = _answerRepository.CalcAnswerWeights(userCount);

            int index = 0;
            foreach (var answerWeight in answerWeights)
            {
                User user = null;
                if (answerWeight.UserId != null)
                {
                    user = await _userRepository.GetUserByIdAsync(answerWeight.UserId);
                }

                result[user?.Name ?? $"unknown {++index}"] = answerWeight.Weight;
            }

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