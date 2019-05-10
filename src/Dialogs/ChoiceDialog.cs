using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace CoreBot.Dialogs
{
    public class ChoiceDialog : CancelAndHelpDialog
    {
        public ChoiceDialog(IScenarioService scenarioService, IUserService userService) : base(nameof(ChoiceDialog), scenarioService, userService)
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            
            var waterfallStep = new WaterfallStep[]
            {
                ShowChoiceDialog,
                AnswerToChoiceDialog
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowChoiceDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("bla-bla-bla choice"),
                    RetryPrompt = MessageFactory.Text("Please choose an option from the list."),
                    Choices = ChoiceFactory.ToChoices(new List<string>(new[] {"op1", "op2"})),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> AnswerToChoiceDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var actualAnswer = (FoundChoice)stepContext.Result;

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"The answer is {actualAnswer.Value}"),
                cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}