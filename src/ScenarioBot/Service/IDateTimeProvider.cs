using System;

namespace ScenarioBot.Service
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow();
    }
}