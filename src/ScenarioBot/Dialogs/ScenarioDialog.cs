using System;
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
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

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
                
            if (scenarioDetails == null)
            {
                var userId = new UserId(stepContext.Context.Activity);

                // либо первый раз запускаем, либо надо дать пользователю шанс выбрать сценарий
                if (scenarioDetails == null)
                {
                    return await stepContext.ReplaceDialogAsync(nameof(ScenarioListDialog), userId, cancellationToken);
                }

                //scenarioDetails = _scenarioService.GetLastScenarioDetailsExceptGameOver(userId, null);

                

                // этот фин ушами чтобы пробросить scenarioDetails - вызывается один раз, когда происходит инициализация 
                return await stepContext.ReplaceDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
            }

            var puzzle = _scenarioService.GetNextPuzzle(scenarioDetails.UserId, scenarioDetails.ScenarioId,
                scenarioDetails.LastPuzzleDetails?.PuzzleId, scenarioDetails.LastPuzzleDetails?.ActualAnswer);
            var puzzleDetails = new PuzzleDetails(puzzle);

            if (puzzleDetails.IsLastPuzzle)
            {
                await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions {Prompt = MessageFactory.Text($"{puzzleDetails.Question}")}, cancellationToken);
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            return await stepContext.BeginDialogAsync(
                puzzleDetails.PuzzleType.ToString(), puzzleDetails, cancellationToken);
        }

        private async Task<DialogTurnResult> Check(WaterfallStepContext stepContext, CancellationToken cancellationToken) {

            var scenarioDetails = (ScenarioDetails)stepContext.Options;
            var puzzleDetails =  (PuzzleDetails)stepContext.Result;
            scenarioDetails.LastPuzzleDetails = puzzleDetails;
            await _userService.SetAnswer(scenarioDetails);

            if(!_scenarioService.IsOver(scenarioDetails.UserId, scenarioDetails.ScenarioId, puzzleDetails.PuzzleId))
            {
                return await stepContext.ReplaceDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(scenarioDetails, cancellationToken);
        }
    }
}