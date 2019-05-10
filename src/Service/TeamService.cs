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

        public async Task AddMember(string teamId, User member)
        {
            if (TryGetTeamId(member) != null)
                throw new InvalidOperationException("User is already member of team.");
            if (!await IsTeamExists(teamId))
                throw new InvalidOperationException("Team not found.");
            await _teamRepository.AddMemberAsync(teamId, new UserId(member.ChannelId, member.UserId));
        }

        public async Task<Team> CreateSingleUserTeam(User user)
        {
            var teamId = Guid.NewGuid().ToString("B", CultureInfo.InvariantCulture);
            return await CreateTeam(teamId, user, TeamType.SingleUser);
        }

        public async Task<Team> CreateTeam(string id, User leader)
        {
            return await CreateTeam(id, leader, TeamType.MultiUser);
        }

        public async Task<Team> CreateTeam(string id, User leader, TeamType type)
        {
            if (await IsTeamExists(id))
                throw new InvalidOperationException("Team already exists.");
            var team = new Team(id, new UserId(leader.ChannelId, leader.UserId))
            {
                TeamType = type
            };
            await _teamRepository.AddTeamAsync(team);
            return team;
        }

        public async Task<bool> IsTeamExists(string id)
        {
            return await _teamRepository.IsTeamExists(id);
        }

        public string TryGetTeamId(User user)
        {
            return user.TeamId;
        }
    }
}
