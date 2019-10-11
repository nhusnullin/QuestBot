using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain;
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

        public ScenarioListDialog(IScenarioService scenarioService) : base(nameof(ScenarioListDialog))
        {
            _scenarioService = scenarioService;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)) {Style = ListStyle.SuggestedAction});

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
            var userId = new UserId(stepContext.Context.Activity);

            var notCompletedScenarioNames = await _scenarioService.GetNotCompletedScenarioNames(userId);

            if (notCompletedScenarioNames.Any())
            {
                await stepContext.Context.SendActivityAsync($"Доступныx сценариев ({notCompletedScenarioNames.Count}):",
                    cancellationToken: cancellationToken);
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Choices = ChoiceFactory.ToChoices(notCompletedScenarioNames)
                    },
                    cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Поздравляем! Вы прошли все сценарии",
                    cancellationToken: cancellationToken);
                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
        }

        private async Task<DialogTurnResult> AnswerToChoiceDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var scenarioId = ((FoundChoice) stepContext.Result).Value;
            var userId = new UserId(stepContext.Context.Activity);

            var scenarioDetails = _scenarioService.GetLastScenarioDetailsExceptGameOver(userId, scenarioId);

            if (scenarioDetails == null)
                scenarioDetails = new ScenarioDetails
                {
                    ScenarioId = scenarioId,
                    UserId = userId
                };

            //var scenarioDetails = _scenarioService.GetLastScenarioDetailsExceptGameOver(teamId, null);
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
                parameters = replyMarkup
            };

            reply.ChannelData = JObject.FromObject(channelData);
        }
    }
}