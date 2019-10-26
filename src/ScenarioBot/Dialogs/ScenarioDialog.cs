using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Dialogs;
using Core.Domain;
using Core.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using ScenarioBot.Domain;
using ScenarioBot.Service;

namespace ScenarioBot.Dialogs
{
    public class ScenarioDialog : CancelAndHelpDialog
    {
        private readonly INotificationService _notificationService;
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;

        public ScenarioDialog(IScenarioService scenarioService,
            IUserService userService,
            INotificationService notificationService,
            IList<IBotCommand> botCommands)
            : base(nameof(ScenarioDialog), botCommands)
        {
            _scenarioService = scenarioService;
            _userService = userService;
            _notificationService = notificationService;

            var waterfallStep = new WaterfallStep[]
            {
                Ask,
                Check
            };
            AddDialog(new WaitTextPuzzleDialog(botCommands, notificationService));
            AddDialog(new TextPuzzleDialog(botCommands));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            AddDialog(new ScenarioListDialog(_scenarioService));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> Ask(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var scenarioDetails = (ScenarioDetails) stepContext.Options;

#pragma warning disable 4014
            _userService.InsertOrMergeAsync(new User(stepContext.Context.Activity));
#pragma warning restore 4014

            var userId = new UserId(stepContext.Context.Activity);
            
            if (scenarioDetails == null)
            {
                // либо первый раз запускаем, либо надо дать пользователю шанс выбрать сценарий
                if (scenarioDetails == null)
                    return await stepContext.ReplaceDialogAsync(nameof(ScenarioListDialog), userId, cancellationToken);

                //scenarioDetails = _scenarioService.GetLastScenarioDetailsExceptGameOver(userId, null);


                // этот фин ушами чтобы пробросить scenarioDetails - вызывается один раз, когда происходит инициализация 
                return await stepContext.ReplaceDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
            }

            scenarioDetails.UserId = userId;
            var puzzle = _scenarioService.GetNextPuzzle(userId, scenarioDetails.ScenarioId,
                scenarioDetails.LastPuzzleDetails?.PuzzleId, scenarioDetails.LastPuzzleDetails?.ActualAnswer);
            var puzzleDetails = new PuzzleDetails(puzzle);

            // грязный хак с пробрасыванием времени, которое проставляется при вопросах с задержкой
            if (scenarioDetails.LastPuzzleDetails != null && puzzleDetails.PuzzleId == scenarioDetails.LastPuzzleDetails.PuzzleId)
            {
                // такой кейс возникает, когда мы пробрасываем  PuzzleDetails из WaitingDialog в методе Check
                // и потом его заменем парой строчек выше
                puzzleDetails.QuestionAskedAt = scenarioDetails.LastPuzzleDetails.QuestionAskedAt;
                puzzleDetails.AnswerTimeNoLessThan = scenarioDetails.LastPuzzleDetails.AnswerTimeNoLessThan;
            }

            if (puzzleDetails.IsLastPuzzle)
            {
                await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions {Prompt = MessageFactory.Text($"{puzzleDetails.Question}")}, cancellationToken);
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            return await stepContext.BeginDialogAsync(
                puzzleDetails.PuzzleType.ToString(), puzzleDetails, cancellationToken);
        }

        private async Task<DialogTurnResult> Check(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var scenarioDetails = (ScenarioDetails) stepContext.Options;
            var puzzleDetails = (PuzzleDetails) stepContext.Result;
            scenarioDetails.LastPuzzleDetails = puzzleDetails;
            scenarioDetails.UserId = new UserId(stepContext.Context.Activity);
            
            await _userService.SetAnswer(scenarioDetails);

            if (!_scenarioService.IsOver(scenarioDetails.ScenarioId, puzzleDetails.PuzzleId))
                return await stepContext.ReplaceDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);

            return await stepContext.EndDialogAsync(scenarioDetails, cancellationToken);
        }
    }
}