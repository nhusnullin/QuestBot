using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Azure.Cosmos.Table;

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

        public async Task UpdateTeamNameAsync(string teamId, string name)
        {
            var table = _cloudStorage.GetOrCreateTable(TableName);
            var teamEntity = await _cloudStorage.RetrieveEntityByAsync<TeamEntity>(table, GetPartitionKey(teamId), teamId);
            teamEntity.Name = name;
            teamEntity.ETag = "*";
            var operation = TableOperation.Merge(teamEntity);
            var result = await table.ExecuteAsync(operation);
        }

        public async Task AddTeamAsync(Team team)
        {
            var entity = new TeamEntity(GetPartitionKey(team.Id), team.Id, team.Name)
            {
                PinCode = team.PinCode
            };
            var table = _cloudStorage.GetOrCreateTable(TableName);
            await _cloudStorage.InsertAsync(table, entity);
            await AddMemberAsync(team.Id, team.Leader, true);
            /*foreach (var member in team.Members)
            {
                await AddMemberAsync(team.Id, member, member.Equals(team.Leader));
            }*/
        }

        public async Task<Team> TryGetTeamByIdAsync(string teamId)
        {
            var table = _cloudStorage.GetOrCreateTable(TableName);
            var teamEntity = await _cloudStorage.RetrieveEntityByAsync<TeamEntity>(table, GetPartitionKey(teamId), teamId);
            var members = await _userRepository.GetTeamUsersAsync(teamId);
            var captain = members.Where(i => i.IsCaptain).Select(i => new UserId(i.ChannelId, i.UserId)).Single();
            var result = new Team(teamEntity.RowKey, teamEntity.Name, teamEntity.PinCode, captain);
            return result;

        }

        public Task<string> TryGetTeamIdByName(string teamName)
        {
            var table = _cloudStorage.GetOrCreateTable(TableName);

            TableQuery<TeamEntity> query =
               new TableQuery<TeamEntity>()
                  .Where(TableQuery.GenerateFilterCondition(nameof(TeamEntity.Name),
                      QueryComparisons.Equal, teamName));
            var result = table.ExecuteQuery(query).SingleOrDefault();
            return Task.FromResult(result?.RowKey);
        }


        public async Task<bool> IsPinExists(int pinCode)
        {
            var result = await TryGetTeamIdByPin(pinCode);
            return result != null;
        }

        public Task<string> TryGetTeamIdByPin(int pinCode)
        {
            var table = _cloudStorage.GetOrCreateTable(TableName);

            TableQuery<TeamEntity> query =
               new TableQuery<TeamEntity>()
                  .Where(TableQuery.GenerateFilterConditionForInt(nameof(TeamEntity.PinCode),
                      QueryComparisons.Equal, pinCode));
            var result = table.ExecuteQuery(query).SingleOrDefault();
            return Task.FromResult(result?.RowKey);
        }

        public Task DeleteTeams()
        {
            _cloudStorage.DeleteTableIfExists(TableName);
            return Task.CompletedTask;
        }

        public async Task<ICollection<Team>> GetTeams()
        {
            var table = _cloudStorage.GetOrCreateTable(TableName);
            var teams = (await _cloudStorage.RetrieveEntitiesAsync<TeamEntity>(table)).ToDictionary(i => i.RowKey, i => i);
            var users = await _userRepository.GetUsersAsync();
            var teamUsers = users.GroupBy(i => i.TeamId, i => i);
            var result = new List<Team>();
            foreach(var teamUser in teamUsers.Where(i => i.Key != null))
            {
                if (!teams.TryGetValue(teamUser.Key, out var foundTeam))
                    continue;
                var team = new Team(teamUser.Key, foundTeam.Name, foundTeam.PinCode,
                        teamUser.Where(i => i.IsCaptain).Select(i => new UserId(i.PartitionKey, i.RowKey)).Single());
                result.Add(team);
            }
            return result;
        }

        private static string GetPartitionKey(string teamId)
        {
            // Use first char for partition key
            return teamId.Substring(0, 1);
        }
    }
}
