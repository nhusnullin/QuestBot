using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain;
using Core.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using ScenarioBot.Domain;
using ScenarioBot.Service;

namespace ScenarioBot.Dialogs
{
    public class ScenarioListDialog : ComponentDialog
    {
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;
        private readonly INotificationMessanger _notificationMessanger;
        private readonly ConcurrentDictionary<UserId, ConversationReference> _conversationReferences;

        public ScenarioListDialog(IScenarioService scenarioService, IUserService userService,
             ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
            INotificationMessanger notificationMessanger) : base(nameof(ScenarioListDialog))
        {
            _scenarioService = scenarioService;
            _userService = userService;
            _conversationReferences = conversationReferences;
            _notificationMessanger = notificationMessanger ?? throw new System.ArgumentNullException(nameof(notificationMessanger));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)) { Style = ListStyle.SuggestedAction });

            var waterfallStep = new WaterfallStep[]
            {
                ShowChoiceDialog,
                AnswerToChoiceDialog
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowChoiceDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var teamId = (string)stepContext.Options;

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Пожалуйста, выберите сценарий:"),
                    RetryPrompt = MessageFactory.Text("Пожалуйста, выберите сценарий :"),
                    Choices = ChoiceFactory.ToChoices(_scenarioService.GetAvailableScenario(teamId)),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> AnswerToChoiceDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var scenarioId = ((FoundChoice)stepContext.Result).Value;
            var teamId = (string)stepContext.Options;

            var scenarioDetails = _userService.GetLastScenarioDetailsExceptGameOver(teamId);

            if (scenarioDetails == null || scenarioDetails.ScenarioId != scenarioId)
            {
                scenarioDetails = new ScenarioDetails()
                {
                    ScenarioId = scenarioId,
                    UserId = teamId
                };
            }
            var replyMessage = $"Выбранный сценарий: {scenarioId}";
            var reply = stepContext.Context.Activity.CreateReply(replyMessage);
            GenerateHideKeybordMarkupForTelegram(reply);
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            //await TeamUtils.SendTeamMessage(_teamService, stepContext.Context, _notificationMessanger, teamId, 
                //replyMessage, _conversationReferences, cancellationToken, false);
            return await stepContext.BeginDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
        }

        

        private void GenerateHideKeybordMarkupForTelegram(IActivity reply)
        {
            var replyMarkup = new
            {
                reply_markup = new
                {
                    hide_keyboard = true
                }
            };

            var channelData = new
            {
                method = "sendMessage",
                parameters = replyMarkup,
            };

            reply.ChannelData = JObject.FromObject(channelData);
        }
    }

}