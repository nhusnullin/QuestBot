using CoreBot.Properties;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class SelectTeamDialog : ComponentDialog
    {
        private readonly ITeamService _teamService;
        private const string newTeamNameDialog = "InputNewTeamNamePromptDialog";
        private const string existsTeamNameDialog = "InputExistsTeamNamePromptDialog";
        public SelectTeamDialog(ITeamService teamService) : base(nameof(SelectTeamDialog))
        {
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(newTeamNameDialog, NewTeamNameValidator));
            AddDialog(new TextPrompt(existsTeamNameDialog, ExistsTeamNameValidator));
            var waterfallStep = new WaterfallStep[]
            {
                SelectTeamTypeStep,
                SelectTeamStep,
                InputTeamNameStep,
                FinishStep
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);

        }

        private async Task<DialogTurnResult> SelectTeamTypeStep(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(Resources.SelectTeamTypeText),
                    RetryPrompt = MessageFactory.Text(Resources.RetryPromptText),
                    Choices = ChoiceFactory.ToChoices(new List<string>(new[] { Resources.OnePlayerTeam, Resources.MultiPlayerTeam })),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> SelectTeamStep(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var choice = (FoundChoice)stepContext.Result;
            if (choice.Value == Resources.OnePlayerTeam)
            {
                var user = GetCurrentUser(stepContext);
                var team = await _teamService.CreateSingleUserTeam(user);
                return await stepContext.EndDialogAsync(team.Id, cancellationToken);
            }

            return await stepContext.PromptAsync(
                nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(Resources.SelectTeamText),
                    RetryPrompt = MessageFactory.Text(Resources.RetryPromptText),
                    Choices = ChoiceFactory.ToChoices(new List<string>(new[] { Resources.JoinTeamText, Resources.CreateTeamText })),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> InputTeamNameStep(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {

            var choice = (FoundChoice)stepContext.Result;
            stepContext.Values["TeamJoinType"] = choice.Value;

            return await stepContext.PromptAsync(
                choice.Value == Resources.CreateTeamText ? newTeamNameDialog : existsTeamNameDialog,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(Resources.InputTeamNameMessage),
                    RetryPrompt = MessageFactory.Text(choice.Value == Resources.CreateTeamText ? Resources.TeamAlreadyExistsMessage : Resources.TeamNotFoundMessage),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> FinishStep(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {

            var teamId = (string)stepContext.Result;
            var joinType = (string)stepContext.Values["TeamJoinType"];
            var user = GetCurrentUser(stepContext);
            if (joinType == Resources.CreateTeamText)
            {
                var team = await _teamService.CreateTeam(teamId, user);
            }
            else
            {
                await _teamService.AddMember(teamId, user);
            }
            return await stepContext.EndDialogAsync(teamId, cancellationToken);
        }

        private async Task<bool> NewTeamNameValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var result = promptContext.Recognized.Value;
            return !await _teamService.IsTeamExists(result);
        }

        private async Task<bool> ExistsTeamNameValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var result = promptContext.Recognized.Value;
            return await _teamService.IsTeamExists(result);
        }

        private User GetCurrentUser(WaterfallStepContext stepContext)
        {
            return (User)stepContext.Options;
        }
    }

}
