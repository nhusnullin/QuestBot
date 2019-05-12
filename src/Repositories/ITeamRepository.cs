using CoreBot.Domain;
using System.Threading.Tasks;

namespace CoreBot.Repositories
{
    public interface ITeamRepository
    {
        Task AddTeamAsync(Team team);
        Task AddMemberAsync(string teamId, UserId user);
        Task<string> TryGetTeamIdByName(string teamName);
        Task<bool> IsPinExists(int pin);
        Task<string> TryGetTeamIdByPin(int pinCode);
        Task UpdateTeamNameAsync(string teamId, string name);
    }
}
