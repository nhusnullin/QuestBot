//using CoreBot.Domain;
//using CoreBot.Properties;
//using CoreBot.Service;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.Dialogs.Choices;
//using Microsoft.Bot.Schema;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//
//namespace CoreBot.Dialogs
//{
//    public class SelectTeamDialog : ComponentDialog
//    {
//        private readonly INotificationMessanger _notificationMessanger;
//        protected ConcurrentDictionary<UserId, ConversationReference> _conversationReferences;
//        private const string teamPinCodeDialog = "TeamPinCode";
//        public SelectTeamDialog(
//            INotificationMessanger notificationMessanger,
//            ConcurrentDictionary<UserId, ConversationReference> conversationReferences) : base(nameof(SelectTeamDialog))
//        {
//            _conversationReferences = conversationReferences ?? throw new System.ArgumentNullException(nameof(conversationReferences));
//            _notificationMessanger = notificationMessanger ?? throw new System.ArgumentNullException(nameof(notificationMessanger));
//
//            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
//            AddDialog(new NumberPrompt<int>(teamPinCodeDialog, TeamPinValidator));
//            var waterfallStep = new WaterfallStep[]
//            {
//                SelectPlayModeStep,
//                SelectTeamStep,
//                FinishStep
//            };
//
//            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
//            InitialDialogId = nameof(WaterfallDialog);
//
//        }
//
//        private async Task<DialogTurnResult> SelectPlayModeStep(WaterfallStepContext stepContext,
//            CancellationToken cancellationToken)
//        {
//            return await stepContext.PromptAsync(
//                nameof(ChoicePrompt),
//                new PromptOptions
//                {
//                    Prompt = MessageFactory.Text(Resources.SelectTeamTypeText),
//                    RetryPrompt = MessageFactory.Text(Resources.RetryPromptText),
//                    Choices = ChoiceFactory.ToChoices(new List<string>(new[] { Resources.CreateTeam, Resources.JoinToTeam })),
//                },
//                cancellationToken);
//        }
//
//        private async Task<DialogTurnResult> SelectTeamStep(WaterfallStepContext stepContext,
//            CancellationToken cancellationToken)
//        {
//            var choice = (FoundChoice)stepContext.Result;
//            if (choice.Value == Resources.CreateTeam)
//            {
//                var user = GetCurrentUser(stepContext);
//                var team = await _teamService.CreateTeam(user);
//                var message = String.Format(CultureInfo.InvariantCulture, Resources.CreateTeamCompletedMessage, team.Name, team.PinCode);
//                var activity = MessageFactory.Text(message, message, InputHints.IgnoringInput);
//                await TurnContextExtensions.SendMessageAsync(stepContext.Context, message, cancellationToken);
//                return await stepContext.EndDialogAsync(team.Id, cancellationToken);
//            }
//
//            return await stepContext.PromptAsync(
//                teamPinCodeDialog,
//                new PromptOptions
//                {
//                    Prompt = MessageFactory.Text(Resources.InputPinTeamMessage),
//                    RetryPrompt = MessageFactory.Text(Resources.PinNotFoundMessage),
//                },
//                cancellationToken);
//        }
//
//        private async Task<DialogTurnResult> FinishStep(WaterfallStepContext stepContext,
//            CancellationToken cancellationToken)
//        {
//            var teamPinCode = (int)stepContext.Result;
//            var user = GetCurrentUser(stepContext);
//            var team = await _teamService.AddMember(teamPinCode, user);
//            var message = String.Format(CultureInfo.InvariantCulture, Resources.WelcomeToTeamMessage, team.Name);
//            await TurnContextExtensions.SendMessageAsync(stepContext.Context, message, cancellationToken);
//            var teamMessage = "К вашей команде присоединился '" + user.Name+"'";
//            //await TeamUtils.SendTeamMessage(_teamService, stepContext.Context, _notificationMessanger, team.Id, teamMessage, _conversationReferences, cancellationToken, false);
//            return await stepContext.EndDialogAsync(team.Id, cancellationToken);
//        }
//
//        private async Task<bool> TeamPinValidator(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
//        {
//            var result = promptContext.Recognized.Value;
//            return await _teamService.IsPinExists(result);
//        }
//
//        private User GetCurrentUser(WaterfallStepContext stepContext)
//        {
//            return (User)stepContext.Options;
//        }
//    }
//
//}
