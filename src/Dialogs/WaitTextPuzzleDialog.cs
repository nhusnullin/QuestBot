using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace CoreBot.Dialogs
{
    public class WaitTextPuzzleDialog : TextPuzzleDialog
    {
        public WaitTextPuzzleDialog(IScenarioService scenarioService, IUserService userService) : base(scenarioService, userService, nameof(WaitTextPuzzleDialog))
        {
        }

        protected override async Task<DialogTurnResult> AskDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;

            if (!puzzleDetails.QuestionAskedAt.HasValue)
            {
                // если первый заход, то задаем вопрос
                puzzleDetails.SetQuestionAskedAt(DateTime.UtcNow);
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions {Prompt = MessageFactory.Text($"{puzzleDetails.Question}")}, cancellationToken);
            }

            var remainMinutesToAnswer = puzzleDetails.GetRemainMinutesToAnswer(DateTime.UtcNow);
            if (remainMinutesToAnswer > 0)
            {
                var text = $"Продолжить прохождение квеста возможно лишь через {remainMinutesToAnswer} мин";
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions {Prompt = MessageFactory.Text(text)}, cancellationToken);
            }

            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        protected override async Task<DialogTurnResult> CheckDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;

            if (puzzleDetails.GetRemainMinutesToAnswer(DateTime.UtcNow) > 0)
            {
                return await stepContext.ReplaceDialogAsync(puzzleDetails.PuzzleType.ToString(), puzzleDetails, cancellationToken);
            }

            return await base.CheckDialog(stepContext, cancellationToken);
        }
    }
}