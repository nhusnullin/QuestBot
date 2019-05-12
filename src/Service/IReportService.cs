using CoreBot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public interface IReportService
    {
        Task<ICollection<TeamQuestResult>> GetTeamQuestResults();
    }
}
