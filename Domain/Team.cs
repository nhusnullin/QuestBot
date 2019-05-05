using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace CoreBot.Domain
{
    public enum TeamType
    {
        SingleUser,
        MultiUser
    }

    public class Team
    {
        public Team (string id, UserId leader)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Leader = leader ?? throw new ArgumentNullException(nameof(leader));
            Members = new HashSet<UserId>
            {
                leader
            };
            TeamType = TeamType.SingleUser;
        }

        public string Id { get; }
        public UserId Leader { get; }
        public ISet<UserId> Members { get; }
        public TeamType TeamType { get; set; }
    }
}
