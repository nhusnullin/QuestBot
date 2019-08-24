using System.Collections.Generic;
using System.Threading.Tasks;
using ScenarioBot.Domain;

namespace CoreBot.Service
{
    public interface IReportService
    {
        Task<ICollection<TeamQuestResult>> GetTeamQuestResults();
    }
}