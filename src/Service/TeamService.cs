using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Repositories;

namespace CoreBot.Service
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        }

        public async Task<Team> AddMember(int pinCode, User member)
        {
            if (TryGetTeamId(member) != null)
                throw new InvalidOperationException("User is already member of team.");
            var teamId = await _teamRepository.TryGetTeamIdByPin(pinCode);
            if (teamId == null)
                throw new InvalidOperationException("Team not found.");
            await _teamRepository.AddMemberAsync(teamId, new UserId(member.ChannelId, member.UserId));
            var team = await _teamRepository.TryGetTeamByIdAsync(teamId);
            return team;
        }

        public async Task<Team> CreateTeam(User leader)
        {
            var teamId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
            var pin = await GetUniqueTeamPinCode();
            var teamName = await GetUniqueTeamName(leader);
            var team = new Team(teamId, teamName, pin, new UserId(leader.ChannelId, leader.UserId));
            await _teamRepository.AddTeamAsync(team);
            return team;
        }

        public async Task<string> TryGetTeamIdByName(string name)
        {
            return await _teamRepository.TryGetTeamIdByName(name);
        }

        public async Task<bool> IsPinExists(int pin)
        {
            return await _teamRepository.IsPinExists(pin);
        }


        public string TryGetTeamId(User user)
        {
            return user.TeamId;
        }

        public async Task ChangeTeamName(string teamId, string name)
        {
            await _teamRepository.UpdateTeamNameAsync(teamId, name);
        }

        public async Task DeleteTeams()
        {
            await _teamRepository.DeleteTeams();
        }

        public async Task<ICollection<Team>> GetTeams()
        {
            return await _teamRepository.GetTeams();
        }

        private async Task<int> GetUniqueTeamPinCode()
        {
            int result = 0;
            do
            {
                result = GeneratePin();
            }
            while (await IsPinExists(result));
            return result;
        }

        private static int GeneratePin()
        {
            const int min = 1000;
            const int max = 9999;
            Random rdm = new Random();
            return rdm.Next(min, max);
        }

        private async Task<string> GetUniqueTeamName(User leader)
        {
            var name = leader.Name;
            const string defaultTeamName = "Team";
            if (String.IsNullOrEmpty(name))
                name = defaultTeamName + "1";
            var result = name;
            uint counter = 1;
            while(await TryGetTeamIdByName(result) != null)
            {
                if (counter >= UInt16.MaxValue)
                {
                    result = Guid.NewGuid().ToString("D");
                    break;
                }
                ++counter;
                result = defaultTeamName + counter.ToString(CultureInfo.InvariantCulture);

            }
            return result;
        }

        public async Task<ISet<UserId>> GetTeamMembers(string teamId)
        {
            return await _teamRepository.GetTeamMembers(teamId);
        }
    }
}
