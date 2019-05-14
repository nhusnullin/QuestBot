using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Exceptions;
using CoreBot.Properties;
using CoreBot.Service;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        protected ConcurrentDictionary<UserId, ConversationReference> _conversationReferences;
        public CancelAndHelpDialog(string id, IScenarioService scenarioService, IUserService userService, ITeamService teamService,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences)
            : base(id)
        {
            _userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
            _teamService = teamService ?? throw new System.ArgumentNullException(nameof(teamService));
            AddDialog(new ScenarioListDialog(scenarioService, userService, teamService, conversationReferences));
            AddDialog(new SetTeamNameDialog(nameof(SetTeamNameDialog), teamService));
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text.ToLowerInvariant().Replace("/", "");
                var userId = new UserId(innerDc.Context.Activity.ChannelId, innerDc.Context.Activity.From.Id);
                try
                {
                    switch (text)
                    {
                        case "scenario":
                            var scUser = await AuthorizationUtils.ValidateCaptainPermission(userId, _userService, _teamService);
                            return await innerDc.BeginDialogAsync(nameof(ScenarioListDialog), scUser.TeamId, cancellationToken);
                        case "set_team_name":
                            var user = await AuthorizationUtils.ValidateCaptainPermission(userId, _userService, _teamService);
                            return await innerDc.BeginDialogAsync(nameof(SetTeamNameDialog), _teamService.TryGetTeamId(user), cancellationToken);
                        case "help":
                        case "?":
                            await innerDc.Context.SendActivityAsync($"Show Help...");
                            return new DialogTurnResult(DialogTurnStatus.Waiting);

                        case "cancel":
                        case "quit":
                            await innerDc.Context.SendActivityAsync($"Выполнение квеста прервано");
                            return await innerDc.CancelAllDialogsAsync(cancellationToken);
                        case "2edaab42-9871-4c9b-8039-fd262121f8e0":
                            await _userService.DeleteUsers();
                            await _teamService.DeleteTeams();
                            break;

                    }
                }
                catch(UserNotFoundException)
                {
                    await TurnContextExtensions.SendMessageAsync(innerDc.Context, Resources.PleaseWaitStartGame, cancellationToken);
                }
                catch(AuthorizationException ex)
                {
                    await TurnContextExtensions.SendMessageAsync(innerDc.Context, ex.Message, cancellationToken);
                }
            }

            return null;
        }
    }
}
