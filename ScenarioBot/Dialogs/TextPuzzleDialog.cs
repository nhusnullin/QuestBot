using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Dialogs;
using CoreBot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using ScenarioBot.Domain;

namespace ScenarioBot.Dialogs
{
    public class TextPuzzleDialog : CancelAndHelpDialog
    {
        public TextPuzzleDialog(IList<IBotCommand> botCommands,
            string id = "TextPuzzleDialog"
            ) 
            : base(id, botCommands)
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            var waterfallStep = new WaterfallStep[]
            {
                AskDialog,
                CheckDialog
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
        }

        protected virtual async Task<DialogTurnResult> AskDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails)stepContext.Options;
            //await TeamUtils.SendTeamMessage(_teamService, stepContext.Context, _notificationMessanger, puzzleDetails.TeamId, puzzleDetails.Question, _conversationReferences, cancellationToken, false);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(puzzleDetails.Question) }, cancellationToken);
        }

        protected virtual async Task<DialogTurnResult> CheckDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;
            var answer = (string)stepContext.Result;
            puzzleDetails.SetAnswer(answer);
            //var teamMessage = $"�� ������� ����� '{answer}'";
            //await TeamUtils.SendTeamMessage(_teamService, stepContext.Context, _notificationMessanger, puzzleDetails.TeamId, teamMessage, _conversationReferences, cancellationToken, false);
            if (puzzleDetails.IsRight)
            {
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            if (puzzleDetails.NumberOfAttempts >= puzzleDetails.NumberOfAttemptsLimit)
            {
                var message = "� ���������, �� ������������ ��� ������� ������ ���������� �����";
                //await TeamUtils.SendTeamMessage(_teamService, stepContext.Context, _notificationMessanger, puzzleDetails.TeamId, message, _conversationReferences, cancellationToken, false);
                await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions { Prompt = MessageFactory.Text(message) }, cancellationToken);
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            if (puzzleDetails.NumberOfAttemptsLimit.HasValue && puzzleDetails.NumberOfAttemptsLimit.Value > 0)
            {
                return await stepContext.ReplaceDialogAsync(puzzleDetails.PuzzleType.ToString(), puzzleDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
        }
    }
}