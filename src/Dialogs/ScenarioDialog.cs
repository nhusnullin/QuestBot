using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using CoreBot.BotCommands;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;

namespace CoreBot.Dialogs
{
    public class ScenarioDialog : CancelAndHelpDialog
    {
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;
        private readonly ConcurrentBag<BackgroundNotifyMsg> _backgroundNotifyMsgsStore;

        public ScenarioDialog(IScenarioService scenarioService, IUserService userService, 
            ConcurrentBag<BackgroundNotifyMsg> backgroundNotifyMsgsStore,
            IList<IBotCommand> botCommands)
            : base(nameof(ScenarioDialog), botCommands)
        {
            var waterfallStep = new WaterfallStep[]
            {
                Ask,
                Check
            };
            AddDialog(new WaitTextPuzzleDialog(botCommands, _backgroundNotifyMsgsStore));
            AddDialog(new TextPuzzleDialog(botCommands));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
            _scenarioService = scenarioService;
            _userService = userService;
            _backgroundNotifyMsgsStore = backgroundNotifyMsgsStore;
        }

        private async Task<DialogTurnResult> Ask(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var scenarioDetails = (ScenarioDetails)stepContext.Options;
            
            //// �������� ��������� �� ������� ��������
            //var scenarioIsOverOnce = _userService.IsScenarioIsOverByTeam(scenarioDetails.TeamId, scenarioDetails.ScenarioId);

            //if (scenarioIsOverOnce)
            //{
            //    await stepContext.PromptAsync(nameof(TextPrompt),
            //        new PromptOptions { Prompt = MessageFactory.Text("�� ���� �������� ��� ���������, ��� �������� ������") }, cancellationToken);
            //    return await stepContext.CancelAllDialogsAsync(cancellationToken);
            //}

            var puzzle = _scenarioService.GetNextPuzzle(scenarioDetails.TeamId, scenarioDetails.ScenarioId, scenarioDetails.LastPuzzleDetails?.PuzzleId, scenarioDetails.LastPuzzleDetails?.ActualAnswer);
            var puzzleDetails = new PuzzleDetails(puzzle, puzzle.PosibleBranches.Select(x => x.Answer).ToList(), scenarioDetails.TeamId);

            if (puzzleDetails.IsLastPuzzle)
            {
                //await TeamUtils.SendTeamMessage(_teamService, stepContext.Context, _notificationMessanger, puzzleDetails.TeamId, puzzleDetails.Question, _conversationReferences, cancellationToken, false);
                await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions { Prompt = MessageFactory.Text($"{puzzleDetails.Question}") }, cancellationToken);
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            return await stepContext.BeginDialogAsync(
                puzzleDetails.PuzzleType.ToString(),
                new PuzzleDetails(puzzle, puzzle.PosibleBranches.Select(x => x.Answer).ToList(), scenarioDetails.TeamId),
                cancellationToken);
        }

        private async Task<DialogTurnResult> Check(WaterfallStepContext stepContext, CancellationToken cancellationToken) {

            var scenarioDetails = (ScenarioDetails)stepContext.Options;
            var puzzleDetails =  (PuzzleDetails)stepContext.Result;
            scenarioDetails.LastPuzzleDetails = puzzleDetails;
            await _userService.SetAnswer(scenarioDetails);

            if(!_scenarioService.IsOver(scenarioDetails.TeamId, scenarioDetails.ScenarioId, puzzleDetails.PuzzleId))
            {
                return await stepContext.ReplaceDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(scenarioDetails, cancellationToken);
        }
    }
}