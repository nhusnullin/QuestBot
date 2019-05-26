//using CoreBot.Domain;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace CoreBot.Repositories
//{
//    public interface ITeamRepository
//    {
//        Task AddTeamAsync(Team team);
//        Task AddMemberAsync(string teamId, UserId user);
//        Task<string> TryGetTeamIdByName(string teamName);
//        Task<bool> IsPinExists(int pin);
//        Task<string> TryGetTeamIdByPin(int pinCode);
//        Task<Team> TryGetTeamByIdAsync(string teamId);
//        Task UpdateTeamNameAsync(string teamId, string name);
//        Task DeleteTeams();
//        Task<ICollection<Team>> GetTeams();
//        Task<ISet<UserId>> GetTeamMembers(string teamId);
//    }
//}
