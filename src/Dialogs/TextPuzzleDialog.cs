using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace CoreBot.Dialogs
{
    public class TextPuzzleDialog : CancelAndHelpDialog
    {
        public TextPuzzleDialog(IScenarioService scenarioService, IUserService userService, string id = "TextPuzzleDialog") : base(id, scenarioService, userService)
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

        protected virtual async Task<DialogTurnResult> AskDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails)stepContext.Options;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text($"{puzzleDetails.Question}") }, cancellationToken);
        }

        protected virtual async Task<DialogTurnResult> CheckDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;
            puzzleDetails.SetAnswer((string) stepContext.Result);

            if (puzzleDetails.IsRight)
            {
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            if (puzzleDetails.NumberOfAttempts >= puzzleDetails.NumberOfAttemptsLimit)
            {
                await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions { Prompt = MessageFactory.Text("К сожалению, вы использовали все попытки ввести правильный ответ") }, cancellationToken);
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
        }
    }
}