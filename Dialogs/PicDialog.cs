using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;

namespace CoreBot.Dialogs
{
    public class ScenarioDialog : CancelAndHelpDialog
    {
        private readonly IScenarioService scenarioService;
        private readonly IUserService userService;

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
            this.scenarioService = scenarioService;
            this.userService = userService;
        }

        private async Task<DialogTurnResult> Ask(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var scenarioDetails = (ScenarioDetails)stepContext.Options;
            var userid = stepContext.Context.Activity.From.Id;

            var puzzle = scenarioService.GetNextPuzzle(userid, scenarioDetails.ScenarioId, scenarioDetails.LastPuzzleDetails?.PuzzleId);

            return await stepContext.BeginDialogAsync(puzzle.PuzzleType.ToString(), new PuzzleDetails(puzzle), cancellationToken);
        }

        private async Task<DialogTurnResult> Check(WaterfallStepContext stepContext, CancellationToken cancellationToken) {

            var scenarioDetails = (ScenarioDetails)stepContext.Options;
            var puzzleDetails =  (PuzzleDetails)stepContext.Result;

            userService.SetAnswer(scenarioDetails.TeamId, puzzleDetails.ScenarioId, puzzleDetails.PuzzleId, puzzleDetails.ActualAnswer);

            if(!scenarioService.IsOver(scenarioDetails.TeamId, scenarioDetails.ScenarioId))
            {
                scenarioDetails.LastPuzzleDetails = puzzleDetails;
                return await stepContext.ReplaceDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }



    public class TextPuzzleDialog : CancelAndHelpDialog
    {
        public TextPuzzleDialog() : base(nameof(TextPuzzleDialog))
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

            if (!puzzleDetails.IsRight && puzzleDetails.WaitUntilReceiveRightAnswer)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"The answer is {puzzleDetails.ActualAnswer}. It's wrong answer"), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(TextPuzzleDialog), puzzleDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
        }
    }

    //public class PicDialog : CancelAndHelpDialog
    //{
    //    public PicDialog() : base(nameof(PicDialog))
    //    {
    //        AddDialog(new TextPrompt(nameof(TextPrompt)));
    //        AddDialog(new ChoicePrompt("SelectGroupCardDialog") { Style = ListStyle.None });

    //        var waterfallStep = new WaterfallStep[]
    //        {
    //            ShowPicDialog,
    //            AnswerToPicDialog
    //        };

    //        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
    //        InitialDialogId = nameof(WaterfallDialog);
    //    }

    //    private async Task<DialogTurnResult> ShowPicDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(new Attachment
    //            {
    //                ContentUrl = "https://images.ctfassets.net/9n3x4rtjlya6/VKpwBdY6CyiM6qowQuEM/a0808abadeec240b989cd7fc8eca285e/_______________________.png?w=240",
    //                ContentType = "image/png",
    //                Name = "imageName",
    //            }
    //        ), cancellationToken);

    //        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("bla-bla-bla question?") }, cancellationToken);
    //    }

    //    private async Task<DialogTurnResult> AnswerToPicDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        var actualAnswer = (string)stepContext.Result;
    //        var expectedAnswer = "fu-fu-fu";

    //        if (actualAnswer != expectedAnswer)
    //        {
    //            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"The answer is {actualAnswer}. FYI: the right answer is {expectedAnswer}"), cancellationToken);
    //            return await stepContext.ReplaceDialogAsync(nameof(PicDialog), null, cancellationToken);
    //        }

    //        return await stepContext.EndDialogAsync(null, cancellationToken);
    //    }
    //}
}