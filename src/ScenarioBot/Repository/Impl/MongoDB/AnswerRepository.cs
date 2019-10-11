using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
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
            if (userId == null)
            {
                return new List<string>();
            }
            
            var completedScenarioIds = Answers
                .Find(x => x.IsLastAnswer && x.RespondentId.Id == userId.Id)
                .Project(x => x.ScenarioId)
                .ToList()
                .Distinct()
                .ToList();

            return completedScenarioIds;
        }

        public async Task AddAnswer(Answer answer)
        {
            await Answers.InsertOneAsync(answer).ConfigureAwait(false);
        }

        public dynamic CalcAnswerWeights(int take)
        {
            var calculatedAnswers = Answers.AsQueryable().Select(x => new
                {
                    x.Weight,
                    x.PuzzleId,
                    x.ScenarioId,
                    x.RespondentId
                })
                .Where(x=>x.RespondentId.Id != null)
                .Distinct()
                .GroupBy(x => x.RespondentId.Id)
                .Select(ag => new
                {
                    UserId = new UserId("", ag.Key), // todo тут channel надо ставить
                    Weight = ag.Sum(x => x.Weight)
                })
                .OrderByDescending(x => x.Weight)
                .Take(take)
                .ToList();

            return calculatedAnswers;
        }

        public async Task<Answer> GetLastAddedAnswerFromNotCompletedScenario(UserId userId, string scenarioId)
        {
            if (userId == null)
            {
                return null;
            }

            var completedScenarioIds = await GetCompletedScenarioIds(userId);

            return Answers
                .Find(a => !a.IsLastAnswer && a.ScenarioId == scenarioId && a.RespondentId.Id == userId.Id &&
                           !completedScenarioIds.Contains(a.ScenarioId)) // не последний ответ сценария
                // из списка не законченных сценариев
                .SortByDescending(x => x.Timestamp)
                .FirstOrDefault();
        }
    }
}