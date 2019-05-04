using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.BotBuilderSamples;

namespace CoreBot.Dialogs
{
    public class ScenarioDialog : CancelAndHelpDialog
    {
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;

        public ScenarioDialog(IScenarioService scenarioService, IUserService userService) : base(nameof(ScenarioDialog))
        {
            var waterfallStep = new WaterfallStep[]
            {
                Ask,
                Check
            };

            AddDialog(new TextPuzzleDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
            _scenarioService = scenarioService;
            _userService = userService;
        }

        private async Task<DialogTurnResult> Ask(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var scenarioDetails = (ScenarioDetails)stepContext.Options;
            var puzzleDetails = (PuzzleDetails)stepContext.Result;

            var userid = stepContext.Context.Activity.From.Id;

            var puzzle = _scenarioService.GetNextPuzzle(scenarioDetails.TeamId, scenarioDetails.ScenarioId, scenarioDetails.LastPuzzleDetails?.PuzzleId, scenarioDetails.LastPuzzleDetails?.IsRight);

            return await stepContext.BeginDialogAsync(puzzle.PuzzleType.ToString(), new PuzzleDetails(puzzle), cancellationToken);
        }

        private async Task<DialogTurnResult> Check(WaterfallStepContext stepContext, CancellationToken cancellationToken) {

            var scenarioDetails = (ScenarioDetails)stepContext.Options;
            var puzzleDetails =  (PuzzleDetails)stepContext.Result;
            scenarioDetails.LastPuzzleDetails = puzzleDetails;

            await _userService.SetAnswer(stepContext.Context.Activity.ChannelId, scenarioDetails.TeamId, puzzleDetails.ScenarioId, puzzleDetails.PuzzleId, scenarioDetails);

            if(!_scenarioService.IsOver(scenarioDetails.TeamId, scenarioDetails.ScenarioId, puzzleDetails.PuzzleId))
            {
                return await stepContext.ReplaceDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(scenarioDetails, cancellationToken);
        }
    }
}