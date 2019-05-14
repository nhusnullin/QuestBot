using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs
{
    public class TextPuzzleDialog : CancelAndHelpDialog
    {
        private readonly INotificationMessanger _notificationMessanger;
        private readonly ITeamService _teamService;
        public TextPuzzleDialog(IScenarioService scenarioService, IUserService userService, ITeamService teamService,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
            INotificationMessanger notificationMessanger,
            string id = "TextPuzzleDialog") 
            : base(id, scenarioService, userService, teamService, conversationReferences, notificationMessanger)
        {
            _notificationMessanger = notificationMessanger;
            _conversationReferences = conversationReferences;
            _teamService = teamService;
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            var waterfallStep = new WaterfallStep[]
            {
                AskDialog,
                CheckDialog
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
        }

        protected virtual async Task<DialogTurnResult> AskDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails)stepContext.Options;
            TeamUtils.SendTeamMessage(_teamService, stepContext.Context, _notificationMessanger, puzzleDetails.TeamId, puzzleDetails.Question, _conversationReferences, cancellationToken, false);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(puzzleDetails.Question) }, cancellationToken);
        }

        protected virtual async Task<DialogTurnResult> CheckDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;
            puzzleDetails.SetAnswer((string) stepContext.Result);

            if (puzzleDetails.IsRight)
            {
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            if (puzzleDetails.NumberOfAttempts >= puzzleDetails.NumberOfAttemptsLimit)
            {
                await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions { Prompt = MessageFactory.Text("К сожалению, вы использовали все попытки ввести правильный ответ") }, cancellationToken);
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            if (puzzleDetails.NumberOfAttemptsLimit.HasValue && puzzleDetails.NumberOfAttemptsLimit.Value > 0)
            {
                return await stepContext.ReplaceDialogAsync(puzzleDetails.PuzzleType.ToString(), puzzleDetails, cancellationToken);
            }

            // hack! такая развязка нужна из за зацикливания если тип WaitTextPuzzleDialog,
            // при этом если ветка else branch и указано кол-во попыток их надо учитывать
            //if (puzzleDetails.PuzzleType == PuzzleType.WaitTextPuzzleDialog)
            {
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            
        }
    }
}