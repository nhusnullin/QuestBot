using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;

namespace ScenarioBot.Repository.Impl.InMemory
{
    public class AnswerRepository:IAnswerRepository
    {

        public AnswerRepository()
        {
        }

        public Task<IList<string>> GetCompletedScenarioNames(UserId userId)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }
    }
}
