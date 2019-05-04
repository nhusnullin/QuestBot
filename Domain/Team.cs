using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace CoreBot.Domain
{
    public class Team : TableEntity
    {
        public Team (string id, string leader)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Leader = leader ?? throw new ArgumentNullException(nameof(leader));
            Members = new HashSet<string>
            {
                leader
            };
            AllowJoin = false;
        }

        public string Id { get; }
        public string Leader { get; }
        public ISet<string> Members { get; }
        public bool AllowJoin { get; set; }
    }
}
