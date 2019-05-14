using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs
{
    public class ScenarioDialog : CancelAndHelpDialog
    {
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;
        public ScenarioDialog(IScenarioService scenarioService, IUserService userService, ITeamService teamService,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
            INotificationMessanger notificationMessanger)
            : base(nameof(ScenarioDialog), scenarioService, userService, teamService, conversationReferences, notificationMessanger)
        {
            var waterfallStep = new WaterfallStep[]
            {
                Ask,
                Check
            };
            AddDialog(new WaitTextPuzzleDialog(scenarioService, userService, teamService, conversationReferences, notificationMessanger));
            AddDialog(new TextPuzzleDialog(scenarioService, userService, teamService, conversationReferences, notificationMessanger));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
            _scenarioService = scenarioService;
            _userService = userService;
        }

        private async Task<DialogTurnResult> Ask(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var scenarioDetails = (ScenarioDetails)stepContext.Options;
            
            //// проверим проходила ли команда сценарий
            //var scenarioIsOverOnce = _userService.IsScenarioIsOverByTeam(scenarioDetails.TeamId, scenarioDetails.ScenarioId);

            //if (scenarioIsOverOnce)
            //{
            //    await stepContext.PromptAsync(nameof(TextPrompt),
            //        new PromptOptions { Prompt = MessageFactory.Text("Вы этот сценарий уже проходили, пож выберите другой") }, cancellationToken);
            //    return await stepContext.CancelAllDialogsAsync(cancellationToken);
            //}

            var puzzle = _scenarioService.GetNextPuzzle(scenarioDetails.TeamId, scenarioDetails.ScenarioId, scenarioDetails.LastPuzzleDetails?.PuzzleId, scenarioDetails.LastPuzzleDetails?.ActualAnswer);
            var puzzleDetails = new PuzzleDetails(puzzle, puzzle.PosibleBranches.Select(x => x.Answer).ToList(), scenarioDetails.TeamId);

            if (puzzleDetails.IsLastPuzzle)
            {
                await TeamUtils.SendTeamMessage(_teamService, stepContext.Context, _notificationMessanger, puzzleDetails.TeamId, puzzleDetails.Question, _conversationReferences, cancellationToken, false);
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