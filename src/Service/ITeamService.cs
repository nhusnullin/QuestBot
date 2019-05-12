using CoreBot.Domain;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public interface ITeamService
    {
        Task<Team> CreateTeam(User leader);
        Task<string> TryGetTeamIdByName(string teamName);
        Task<bool> IsPinExists(int pin);
        string TryGetTeamId(User user);
        Task<string> AddMember(int pinCode, User member);
        Task ChangeTeamName(string teamId, string name);
        Task DeleteTeams();
    }
}
