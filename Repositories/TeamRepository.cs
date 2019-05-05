using System;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Service;

namespace CoreBot.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly ICloudStorage _cloudStorage;
        private const string TableName = "teams";
        public TeamRepository(IUserRepository userRepository, ICloudStorage cloudStorage)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _cloudStorage = cloudStorage ?? throw new ArgumentNullException(nameof(cloudStorage));
        }

        public async Task AddMemberAsync(string teamId, UserId userId)
        {
            await AddMemberAsync(teamId, userId, false);
        }

        private async Task AddMemberAsync(string teamId, UserId userId, bool isCaptain)
        {
            var user = await _userRepository.TryGetUserByIdAsync(userId);
            if (user.TeamId != teamId || user.IsCaptain != isCaptain)
            {
                user.TeamId = teamId;
                user.IsCaptain = isCaptain;
                await _userRepository.InsertOrMergeAsync(user);
            }
        }

        public async Task AddTeamAsync(Team team)
        {
            var entity = new TeamEntity(GetPartitionKey(team.Id), team.Id)
            {
                IsSingleUser = team.TeamType == TeamType.SingleUser
            };
            var table = _cloudStorage.GetOrCreateTable(TableName);
            await _cloudStorage.InsertAsync(table, entity);

            foreach (var member in team.Members)
            {
                await AddMemberAsync(team.Id, member, member.Equals(team.Leader));
            }
        }

        /*public async Task<Team> TryGetTeamAsync(string teamId)
        {
            var table = _cloudStorage.GetOrCreateTable(TableName);
            var teamEntity = await _cloudStorage.RetrieveEntityByAsync<TeamEntity>(table, GetPartitionKey(teamId), teamId);
            var members = await _userRepository.GetTeamUsersAsync(teamId);
            var captain = members.Where(i => i.IsCaptain).Select(i => new UserId(i.ChannelId, i.UserId)).Single();
            var result = new Team(teamId, captain);
            foreach (var member in members)
                result.Members.Add(new UserId(member.ChannelId, member.UserId));
            return result;

        }*/

        public async Task<bool> IsTeamExists(string teamId)
        {
            var table = _cloudStorage.GetOrCreateTable(TableName);
            var teamEntity = await _cloudStorage.RetrieveEntityByAsync<TeamEntity>(table, GetPartitionKey(teamId), teamId);
            return teamEntity != null;
        }

        private static string GetPartitionKey(string teamId)
        {
            // Use first char for partition key
            return teamId.Substring(0, 1);
        }

    }
}
