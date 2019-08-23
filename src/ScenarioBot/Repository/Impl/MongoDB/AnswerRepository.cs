using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using Microsoft.Recognizers.Text.Number;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ScenarioBot.Domain;

namespace ScenarioBot.Repository.Impl.MongoDB
{
    public class AnswerRepository : MongoConfiguration, IAnswerRepository
    {
        public AnswerRepository(IMongoClient client) : base(client)
        {
        }

        public async Task<IList<string>> GetCompletedScenarioIds(UserId userId)
        {
            return Answers.AsQueryable().Where(x => x.IsLastAnswer)
                .GroupBy(x => x.ScenarioId)
                .SelectMany(x => x.ToList())
                .Select(x => x.ScenarioId)
                .ToList();
        }

        public async Task AddAnswer(Answer answer)
        {
            await Answers.InsertOneAsync(answer).ConfigureAwait(false);
        }

        public void CalcAnswerWeights(int take)
        {

            var calculatedAnswers = Answers.AsQueryable().Select(x => new
            {
                x.Weight,
                x.PuzzleId,
                x.ScenarioId,
                x.RespondentId
            })
                .Distinct()
                .GroupBy(x => x.RespondentId)
                .Select(ag => new
                {
                    UserId = ag.Key.Id,
                    Weight = ag.Sum(x => x.Weight)
                })
                .OrderByDescending(x => x.Weight)
                .Take(take)
                .ToList();
        }

        public Answer GetLastAddedAnswerFromNotCompletedScenario()
        {
            var completedScenarioIds = Answers.AsQueryable().Where(x => x.IsLastAnswer).Select(x => x.ScenarioId).Distinct();

            return Answers.AsQueryable().Where(x => !x.IsLastAnswer) // не последний ответ сценария
                .Where(x => !completedScenarioIds.Contains(x.ScenarioId)) // из списка не законченных сценариев
                .OrderBy(x => x.Timestamp)
                .FirstOrDefault();
        }
    }
}
