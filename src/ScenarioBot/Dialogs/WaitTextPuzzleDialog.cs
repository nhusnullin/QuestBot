using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Domain;
using Core.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using ScenarioBot.Domain;

namespace ScenarioBot.Dialogs
{
    public class WaitTextPuzzleDialog : TextPuzzleDialog
    {
        private readonly INotificationService _notificationService;

        public WaitTextPuzzleDialog(IList<IBotCommand> botCommands,
            INotificationService notificationService)
            : base(botCommands, nameof(WaitTextPuzzleDialog))
        {
            _notificationService = notificationService;
        }

        protected override async Task<DialogTurnResult> AskDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;

            if (!puzzleDetails.QuestionAskedAt.HasValue)
            {
                // если первый заход, то задаем вопрос
                puzzleDetails.SetQuestionAskedAt(DateTime.UtcNow);

                // ставим себе напоминалку что надо сообщить команде о возможном продолжении квеста
                await _notificationService.SendMessageInBackground(new BackgroundNotifyMsg
                {
                    //TeamId = puzzleDetails.TeamId,
                    Msg = "Штрафное время закончилось, можно продолжить квест. Успехов и удачи! :)",
                    WhenByUTC = puzzleDetails.AnswerTimeNoLessThan.AddMinutes(1)
                });

                return await base.AskDialog(stepContext, cancellationToken);
            }

            var remainMinutesToAnswer = puzzleDetails.GetRemainMinutesToAnswer(DateTime.UtcNow);
            if (remainMinutesToAnswer > 0)
            {
                var text = $"Продолжить прохождение квеста возможно лишь через {remainMinutesToAnswer} мин";
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions {Prompt = MessageFactory.Text(text)}, cancellationToken);
            }

            return await base.AskDialog(stepContext, cancellationToken);
        }

        protected override async Task<DialogTurnResult> CheckDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;

            if (puzzleDetails.GetRemainMinutesToAnswer(DateTime.UtcNow) > 0)
                return await stepContext.ReplaceDialogAsync(puzzleDetails.PuzzleType.ToString(), puzzleDetails,
                    cancellationToken);

            return await base.CheckDialog(stepContext, cancellationToken);
        }
    }
}