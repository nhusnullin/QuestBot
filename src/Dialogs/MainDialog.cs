using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using ScenarioBot.Service;

namespace CoreBot.Dialogs
{
    public class MainDialog : CancelAndHelpDialog
    {
        protected readonly ILogger _logger;
        private readonly IUserService _userService;

        public MainDialog(
            ILogger<MainDialog> logger, 
            IList<IBotCommand> botCommands)
            : base(nameof(MainDialog), botCommands)
        {
            _logger = logger;
//            _userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
//            AddDialog(new SelectTeamDialog(teamService, notificationMessanger, conversationReferences));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                //IntroStepAsync,
                //ScenarioLaunchStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            
            // это на тот случай что человек уже себе поставил бота, но пользователя нет у нас в БД
            //await _userService.GetOrCreateUser(stepContext.Context);
            return await stepContext.NextAsync(null, cancellationToken);
        }

//        private async Task<DialogTurnResult> SelectTeamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var user = await _userService.GetByAsync(stepContext.Context.Activity.ChannelId, stepContext.Context.Activity.From.Id);
//            var teamId = _teamService.TryGetTeamId(user);
//            if (teamId != null)
//                return await stepContext.NextAsync(teamId, cancellationToken);
//            return await stepContext.BeginDialogAsync(nameof(SelectTeamDialog), user, cancellationToken);
//        }

//        private async Task<DialogTurnResult> ScenarioLaunchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//        {
//            var teamId = (string)stepContext.Result;
//
//            var scenarioDetails = _userService.GetLastScenarioDetailsExceptGameOver(teamId);
//            var user = await _userService.GetByAsync(stepContext.Context.Activity.ChannelId, stepContext.Context.Activity.From.Id);
////            if (!user.IsCaptain)
////            {
////                await TurnContextExtensions.SendMessageAsync(stepContext.Context, Resources.TeamNotificationInfo, cancellationToken);
////                return await stepContext.EndDialogAsync(null, cancellationToken);
////            }
//            if (scenarioDetails == null)
//            {
//                scenarioDetails = new ScenarioDetails()
//                {
//                    ScenarioId = "testScenario",
//                    TeamId = teamId
//                };
//            }
//
//            return await stepContext.BeginDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
//        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Квест окончен!"), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
