using CoreBot.Domain;
using CoreBot.Properties;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
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
        public SetTeamNameDialog(string dialogId, ITeamService teamService) : base(dialogId)
        {
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
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
