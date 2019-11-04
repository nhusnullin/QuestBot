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
        
        private readonly FindOptions<Answer> _findOptions = new FindOptions<Answer>
            {Collation = new Collation("en", strength: CollationStrength.Primary)};
        
        public AnswerRepository(IMongoClient client) : base(client)
        {
        }

        public async Task<IEnumerable<string>> GetCompletedScenarioIds(UserId userId)
        {
            if (userId == null)
            {
                return new List<string>();
            }

            var completedScenarioIds = Answers
                .Find(x => x.IsLastAnswer && x.RespondentId.Id == userId.Id)
                .Project(x => x.ScenarioId)
                .ToList()
                .Distinct();

            return completedScenarioIds;
        }

        public async Task<bool> IsScenarioCompletedByAsync(UserId userId, string scenarioId)
        {
            if (userId == null)
            {
                return true;
            }
            var completedCollection = await Answers
                .FindAsync(x => x.IsLastAnswer &&
                                x.RespondentId.Id == userId.Id &&
                                x.ScenarioId == scenarioId, _findOptions);

            return await completedCollection.AnyAsync();
        }

        public void RemoveBy(UserId userId)
        {
            Answers.DeleteMany(answer => answer.RespondentId.Id == userId.Id);
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