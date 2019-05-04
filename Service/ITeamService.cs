using CoreBot.Domain;
using System.Threading.Tasks;

namespace CoreBot.Service
{
    public interface ITeamService
    {
        Task<Team> TryGetTeam(string id);
        Task<Team> CreateTeam(string id, User leader);
        Task AddMember(Team team, User member);
        Task<Team> FindTeamByUser(User member);
    }
}
