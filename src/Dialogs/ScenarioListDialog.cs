using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Newtonsoft.Json.Linq;

namespace CoreBot.Dialogs
{
    public class ScenarioListDialog : ComponentDialog
    {
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;

        public ScenarioListDialog(IScenarioService scenarioService, IUserService userService) : base(nameof(ScenarioListDialog))
        {
            _scenarioService = scenarioService;
            _userService = userService;
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
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Пожалуйста, выберите сценарий:"),
                    RetryPrompt = MessageFactory.Text("Пожалуйста, выберите сценарий :"),
                    Choices = ChoiceFactory.ToChoices(new List<string>(_scenarioService.AvailableScenario)),
                    
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> AnswerToChoiceDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var actualAnswer = ((FoundChoice)stepContext.Result).Value;
            
            var userId = stepContext.Context.Activity.From.Id;
            var channelId = stepContext.Context.Activity.ChannelId;
            
            var user = await _userService.GetByAsync(channelId, userId);
            var scenarioDetails = new ScenarioDetails()
            {
                ScenarioId = actualAnswer,
                TeamId = user.TeamId
            };

            var reply = stepContext.Context.Activity.CreateReply($"Выбранный сценарий: {actualAnswer}");
            GenerateHideKeybordMarkupForTelegram(reply);
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

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