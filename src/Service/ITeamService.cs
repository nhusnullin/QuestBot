using CoreBot.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public interface ITeamService
    {
        Task<Team> CreateTeam(User leader);
        Task<string> TryGetTeamIdByName(string teamName);
        Task<bool> IsPinExists(int pin);
        string TryGetTeamId(User user);
        Task<Team> AddMember(int pinCode, User member);
        Task ChangeTeamName(string teamId, string name);
        Task DeleteTeams();
        Task<ICollection<Team>> GetTeams();
    }
}
