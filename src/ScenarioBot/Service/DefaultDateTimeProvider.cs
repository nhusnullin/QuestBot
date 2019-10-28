using System;

namespace ScenarioBot.Service
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}