using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using ScenarioBot.Domain;

namespace ScenarioBot.Repository.Impl.InMemory
{
    public class AnswerRepositoryInMemory:IAnswerRepository
    {
        private readonly List<Answer> _store;
        
        public AnswerRepositoryInMemory()
        {
            _store = new List<Answer>();
        }

        public async Task<IList<string>> GetCompletedScenarioIds(UserId userId)
        {
            return _store.Where(x => x.IsLastAnswer)
                .GroupBy(x => x.ScenarioId)
                .SelectMany(x => x.ToList())
                .Select(x => x.ScenarioId)
                .ToList();
        }

        public async Task AddAnswer(Answer answer)
        {
            _store.Add(answer);
        }

        public Answer GetLastAddedAnswerFromNotCompletedScenario()
        {
            var completedScenarioIds = _store.Where(x => x.IsLastAnswer).Select(x => x.ScenarioId).Distinct();

            return _store.Where(x => !x.IsLastAnswer) // не последний ответ сценария
                .Where(x => !completedScenarioIds.Contains(x.ScenarioId)) // из списка не законченных сценариев
                .OrderBy(x => x.Timestamp)
                .FirstOrDefault();
        }
    }
}
