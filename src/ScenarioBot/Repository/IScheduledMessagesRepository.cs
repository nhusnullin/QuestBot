using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ScenarioBot.Repository
{
    public interface IScheduledMessagesRepository
    {
        Task<ReadOnlyCollection<ScheduledMessage>> GetMessages(DateTime periodStart, DateTime periodEnd);
        Task<ScheduledMessage> GetLastSentMessage();
        Task SetMessageSentDate(ScheduledMessage message, DateTime sentDate);
        Task Add(ScheduledMessage scheduledMessage);
    }
}