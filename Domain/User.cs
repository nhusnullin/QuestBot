namespace CoreBot
{
    /// <summary>
    /// Данные пользователя для хранения в БД 
    /// </summary>
    public class User
    {
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public string Name { get; set; }

        public string TeamId { get; set; }

        public bool IsCaptain { get; set; }
    }
}