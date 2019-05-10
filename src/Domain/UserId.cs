using System;

namespace CoreBot.Domain
{
    public class UserId : IEquatable<UserId>
    {
        public UserId(string channelId, string id)
        {
            ChannelId = channelId ?? throw new ArgumentNullException(nameof(channelId));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string ChannelId { get; }
        public string Id { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserId)obj);
        }

        public bool Equals(UserId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ChannelId == other.ChannelId && Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return ChannelId.GetHashCode() ^ Id.GetHashCode();
        }
    }
}
