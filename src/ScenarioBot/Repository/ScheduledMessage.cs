using System;
using Microsoft.Bot.Schema;

namespace ScenarioBot.Repository
{
    public class ScheduledMessage
    {
        public Guid Id { get; set; }
        public DateTime WhenToSend { get; set; }
        public string Text { get; set; }
        public DateTime? Sent { get; set; }
        public ConversationReference ConversationReference { get; set; }
    }
}