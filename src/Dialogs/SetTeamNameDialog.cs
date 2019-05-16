using CoreBot.Domain;
using CoreBot.Properties;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class SetTeamNameDialog : ComponentDialog
    {
        private const string newTeamNameDialog = "InputNewTeamNamePromptDialog";
        private readonly ITeamService _teamService;
        private readonly INotificationMessanger _notificationMessanger;
        protected ConcurrentDictionary<UserId, ConversationReference> _conversationReferences;
        public SetTeamNameDialog(string dialogId, ITeamService teamService,
            INotificationMessanger notificationMessanger,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences
            ) : base(dialogId)
        {
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _conversationReferences = conversationReferences ?? throw new System.ArgumentNullException(nameof(conversationReferences));
            _notificationMessanger = notificationMessanger ?? throw new System.ArgumentNullException(nameof(notificationMessanger));

            AddDialog(new TextPrompt(newTeamNameDialog, NewTeamNameValidator));
            var waterfallStep = new WaterfallStep[]
            {
                InputTeamNameStep,
                FinishStep
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InputTeamNameStep(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                newTeamNameDialog,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(Resources.InputTeamNameMessage),
                    RetryPrompt = MessageFactory.Text(Resources.TeamAlreadyExistsMessage),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> FinishStep(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var teamName = (string)stepContext.Result;
            var teamId = (string)stepContext.Options;
            await _teamService.ChangeTeamName(teamId, teamName);
            var message = String.Format(CultureInfo.InvariantCulture, Resources.ChangeTeamNameCompletesdMessage, teamName);
            //await TeamUtils.SendTeamMessage(_teamService, stepContext.Context, _notificationMessanger, teamId, message, _conversationReferences, cancellationToken, false);
            await TurnContextExtensions.SendMessageAsync(stepContext.Context, message, cancellationToken);
            return await stepContext.EndDialogAsync(teamName, cancellationToken);
        }

        private async Task<bool> NewTeamNameValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var result = promptContext.Recognized.Value.Trim();
            var teamId = await _teamService.TryGetTeamIdByName(result);
            return teamId == null;
        }

    }
}
