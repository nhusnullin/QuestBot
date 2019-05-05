using CoreBot.Domain;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public interface ITeamService
    {
        Task<Team> CreateTeam(string id, User leader);
        Task<Team> CreateSingleUserTeam(User user);
        Task<bool> IsTeamExists(string id);
        string TryGetTeamId(User user);
        Task AddMember(string teamId, User member);
    }
}
