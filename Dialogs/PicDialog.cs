using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;

namespace CoreBot.Dialogs
{
    public class PicDialog : CancelAndHelpDialog
    {
        public PicDialog() : base(nameof(PicDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt("SelectGroupCardDialog") { Style = ListStyle.None });

            var waterfallStep = new WaterfallStep[]
            {
                ShowPicDialog,
                AnswerToPicDialog
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowPicDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(new Attachment
                {
                    ContentUrl = "https://images.ctfassets.net/9n3x4rtjlya6/VKpwBdY6CyiM6qowQuEM/a0808abadeec240b989cd7fc8eca285e/_______________________.png?w=240",
                    ContentType = "image/png",
                    Name = "imageName",
                }
            ), cancellationToken);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("bla-bla-bla question?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> AnswerToPicDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actualAnswer = (string)stepContext.Result;
            var expectedAnswer = "fu-fu-fu";

            if (actualAnswer != expectedAnswer)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"The answer is {actualAnswer}. FYI: the right answer is {expectedAnswer}"), cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(PicDialog), null, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}