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

        public ScenarioDialog(IScenarioService scenarioService, 
            IUserService userService, 
            INotificationService notificationService,
            IList<IBotCommand> botCommands)
            : base(nameof(ScenarioDialog), botCommands)
        {
            var waterfallStep = new WaterfallStep[]
            {
                Ask,
                Check
            };
            AddDialog(new WaitTextPuzzleDialog(botCommands, notificationService));
            AddDialog(new TextPuzzleDialog(botCommands));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
            _scenarioService = scenarioService;
            _userService = userService;
        }

        private async Task<DialogTurnResult> Ask(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var scenarioDetails = (ScenarioDetails) stepContext.Options;

            if (scenarioDetails == null)
            {
                var id = stepContext.Context.Activity.From.Id;
                var channelId = stepContext.Context.Activity.ChannelId;
                scenarioDetails = _userService.GetLastScenarioDetailsExceptGameOver(new UserId(channelId, id));

                if (scenarioDetails == null)
                {
                    throw new ApplicationException("There is no any scenario to launch");
                }

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