using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;
using ScenarioBot.Domain;

namespace ScenarioBot.Repository
{
    public interface IAnswerRepository
    {
        Task<IList<string>> GetCompletedScenarioIds(UserId userId);

        Task AddAnswer(Answer answer);
    }
}