using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace CoreBot.Dialogs
{
    public class TextPuzzleDialog : CancelAndHelpDialog
    {
        public TextPuzzleDialog() : base(PuzzleType.TextPuzzleDialog.ToString())
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

        private async Task<DialogTurnResult> AskDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails)stepContext.Options;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text($"{puzzleDetails.Question}") }, cancellationToken);
        }

        private async Task<DialogTurnResult> CheckDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails)stepContext.Options;
            puzzleDetails.ActualAnswer = (string)stepContext.Result;

            if (!puzzleDetails.IsRight && puzzleDetails.WaitUntilReceiveRightAnswer.HasValue && puzzleDetails.WaitUntilReceiveRightAnswer.Value)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"The answer is {puzzleDetails.ActualAnswer}. It's wrong answer"), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(TextPuzzleDialog), puzzleDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
        }
    }
}