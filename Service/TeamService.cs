using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Domain;

namespace CoreBot.Service
{
    public class TeamService : ITeamService
    {
        private readonly List<Team> _teams = new List<Team>();

        public async Task AddMember(Team team, User member)
        {
            var foundTeam = await TryGetTeam(team.Id);
            if (foundTeam == null)
                throw new InvalidOperationException("Team not found.");
            foreach (var item in _teams.Where(i => i.Id != team.Id))
                item.Members.Remove(member.UserId);

            if (!team.Members.Contains(member.UserId))
                team.Members.Add(member.UserId);
            return;
        }

        public async Task<Team> CreateTeam(string id, User leader)
        {
            var team = await TryGetTeam(id);
            if (team != null)
                throw new InvalidOperationException("Team already exists.");
            var result = new Team(id, leader.UserId);
            _teams.Add(result);
            return result;
        }

        public async Task<Team> TryGetTeam(string id)
        {
            return _teams.SingleOrDefault(i => i.Id == id);
        }

        public async Task<Team> FindTeamByUser(User member)
        {
            return _teams.Where(i => i.Members.Contains(member.UserId)).SingleOrDefault();
        }
    }
}
