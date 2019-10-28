using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ScenarioBot.Repository.Impl.InMemory
{
    public class ScheduledMessagesRepositoryInMemory : IScheduledMessagesRepository
    {
        private static readonly Dictionary<Guid, ScheduledMessage> Messages = new Dictionary<Guid, ScheduledMessage>();
        private static readonly object Locker = new object();
        
        public Task<ReadOnlyCollection<ScheduledMessage>> GetMessages(DateTime periodStart, DateTime periodEnd)
        {
            lock (Locker)
            {
                var messages = Messages
                    .Select(m => m.Value)
                    .Where(m => !m.Sent.HasValue && m.WhenToSend < periodEnd && m.WhenToSend > periodStart)
                    .ToList();

                return Task.FromResult(new ReadOnlyCollection<ScheduledMessage>(messages));
            }
        }

        public Task SetMessageSentDate(ScheduledMessage message, DateTime sentDate)
        {
            lock (Locker)
            {
                Messages.TryGetValue(message.Id, out message);
                if (message == default)
                    return Task.CompletedTask;

                message.Sent = sentDate;
                return Task.CompletedTask;
            }
        }

        public Task<ScheduledMessage> GetLastSentMessage()
        {
            lock (Locker)
            {
                var lastSentMessage = Messages
                    .Select(m => m.Value)
                    .Where(m => m.Sent.HasValue)
                    .OrderByDescending(m => m.Sent)
                    .FirstOrDefault();

                return Task.FromResult(lastSentMessage);
            }
        }

        public Task Add(ScheduledMessage scheduledMessage)
        {
            lock (Locker)
            {
                scheduledMessage.Id = Guid.NewGuid();
                Messages.Add(scheduledMessage.Id, scheduledMessage);
                return Task.CompletedTask;
            }
        }
    }
}