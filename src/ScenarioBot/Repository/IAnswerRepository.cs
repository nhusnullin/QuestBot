using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain;

namespace ScenarioBot.Repository
{
    public interface IAnswerRepository
    {
        Task<IList<string>> GetCompletedScenarioNames(UserId userId);
    }
}