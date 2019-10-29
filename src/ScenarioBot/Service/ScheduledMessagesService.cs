using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Core.Service;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using ScenarioBot.Repository;

namespace ScenarioBot.Service
{
    public class ScheduledMessagesService : IScheduledMessagesService
    {
        private readonly IScheduledMessagesRepository _messagesRepository;
        private readonly INotificationService _notificationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<ScheduledMessagesService> _logger;
        private static readonly TimeSpan DefaultMessageSearchPeriod = TimeSpan.FromHours(1);

        public ScheduledMessagesService(IScheduledMessagesRepository messagesRepository,
            INotificationService notificationService,
            IDateTimeProvider dateTimeProvider,
            ILogger<ScheduledMessagesService> logger)
        {
            _messagesRepository = messagesRepository;
            _notificationService = notificationService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }
        
        public async Task SendScheduled(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Started sending scheduled message");
            var messagesToSend = await GetMessagesToSend();
            _logger.LogInformation($"Found {messagesToSend.Count} scheduled messages");
            
            foreach (var message in messagesToSend)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _notificationService.SendMessage(message.Text, message.ConversationReference, cancellationToken);
                await _messagesRepository.SetMessageSentDate(message, _dateTimeProvider.UtcNow());
            }
        }

        private async Task<ReadOnlyCollection<ScheduledMessage>> GetMessagesToSend()
        {
            var now = _dateTimeProvider.UtcNow();
            var lastSentTime = await GetLastSentTime();
            
            _logger.LogInformation($"Searching messages for period {lastSentTime.ToLongTimeString()} - {now.ToLongTimeString()}");

            return await _messagesRepository
                .GetMessages(lastSentTime, now);

            async Task<DateTime> GetLastSentTime()
            {
                var lastSentMessage = await _messagesRepository.GetLastSentMessage();
                return lastSentMessage?.Sent ?? now - DefaultMessageSearchPeriod;
            }
        }

        public Task Schedule(string messageText, ConversationReference conversationReference, TimeSpan delay)
        {
            _logger.LogInformation($"Scheduling message for user '{conversationReference.User.Id}' with delay '{delay}'");
            var now = _dateTimeProvider.UtcNow();
            return _messagesRepository.Add(new ScheduledMessage
            {
                Text = messageText,
                ConversationReference = conversationReference,
                Scheduled = now + delay,
                Created = now
            });
        }
    }
}