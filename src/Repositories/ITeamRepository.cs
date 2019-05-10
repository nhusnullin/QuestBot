using CoreBot.Domain;
using System.Threading.Tasks;

namespace CoreBot.Repositories
{
    public interface ITeamRepository
    {
        Task AddTeamAsync(Team team);
        Task AddMemberAsync(string teamId, UserId user);
        Task<bool> IsTeamExists(string teamId);
    }
}
