using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Domain;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Core.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        private readonly IList<IBotCommand> _botCommands;

        public CancelAndHelpDialog(string id, IList<IBotCommand> botCommands): base(id)
        {
            _botCommands = botCommands;
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

        private async Task<DialogTurnResult> InterruptAsync(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            if (dialogContext.Context.Activity.Type != ActivityTypes.Message)
            {
                return null;
            }
            
            var text = dialogContext.Context.Activity.Text.ToLowerInvariant().Replace("/", "");
            var userId = new UserId(dialogContext.Context.Activity.ChannelId, dialogContext.Context.Activity.From.Id);

            var cmd = _botCommands.FirstOrDefault(x => x.IsApplicable(text, userId));

            if (cmd == null)
            {
                return null;
            }
            
            if (!cmd.Validate(userId))
            {
                await dialogContext.Context.SendActivityAsync($"Command is not allowed", cancellationToken: cancellationToken);
                return null;
            }

            foreach (var componentDialog in cmd.GetComponentDialogs())
            {
                AddDialog(componentDialog);
            }
            
            return await cmd.ExecuteAsync(dialogContext, userId, cancellationToken);
            
//            try
//            {
//                switch (text)
//                {
//                    case "scenario":
//                        var scUser = await AuthorizationUtils.ValidateCaptainPermission(userId, _userService, _teamService);
//                        return await dialogContext.BeginDialogAsync(nameof(ScenarioListDialog), scUser.TeamId, cancellationToken);
//                    case "set_team_name":
//                        var user = await AuthorizationUtils.ValidateCaptainPermission(userId, _userService, _teamService);
//                        return await dialogContext.BeginDialogAsync(nameof(SetTeamNameDialog), _teamService.TryGetTeamId(user), cancellationToken);
//                    case "help":
//                    case "?":
//                        await dialogContext.Context.SendActivityAsync($"Show Help...");
//                        return new DialogTurnResult(DialogTurnStatus.Waiting);
//
//                    case "cancel":
//                    case "quit":
//                        await dialogContext.Context.SendActivityAsync($"Выполнение квеста прервано");
//                        return await dialogContext.CancelAllDialogsAsync(cancellationToken);
//                    //case "2edaab42-9871-4c9b-8039-fd262121f8e0":
//                    //    await _userService.DeleteUsers();
//                    //    await _teamService.DeleteTeams();
//                    //break;
//                    case "top10":
//                        await dialogContext.BeginDialogAsync(nameof(ShowRatingDialog), null, cancellationToken);
//                        return new DialogTurnResult(DialogTurnStatus.Waiting);
//                    default:
//                        if (text.StartsWith("top"))
//                        {
//                            var words = text.Split(" ").Where(x => !string.IsNullOrEmpty(x));
//
//                            int.TryParse(words.Last(), out var count);
//
//                            await ShowRating(dialogContext.Context, count, cancellationToken);
//                            return new DialogTurnResult(DialogTurnStatus.Waiting);
//                        }
//
//                        if (text.StartsWith("send"))
//                        {
//                            var words = text.Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToList();
//                            string id = null;
//                            string msg = "";
//
//                            for(int i=0;i<words.Count; i++)
//                            {
//                                if(i == 1)
//                                {
//                                    id = words[i];
//                                }
//
//                                if(i > 1)
//                                {
//                                    msg += " " + words[i];
//                                }
//                            }
//
//                            await SendTeamMessage(_notificationMessanger, msg, cancellationToken, id);
//                            return new DialogTurnResult(DialogTurnStatus.Waiting);
//                        }
//
//                        return null;
//                }
//            }
//            catch(UserNotFoundException)
//            {
//                await TurnContextExtensions.SendMessageAsync(dialogContext.Context, Resources.PleaseWaitStartGame, cancellationToken);
//                return new DialogTurnResult(DialogTurnStatus.Waiting);
//            }
//            catch(AuthorizationException ex)
//            {
//                await TurnContextExtensions.SendMessageAsync(dialogContext.Context, ex.Message, cancellationToken);
//                return new DialogTurnResult(DialogTurnStatus.Waiting);
//            }
//
//            return null;
//
//
//            var v = 1 + 1;
        }
//
//        public async Task SendTeamMessage(
//            INotificationMessanger messenger,
//            string message,
//            CancellationToken cancellationToken,
//            string userId
//        )
//        {
//            if (string.Equals(userId, "toAll", StringComparison.CurrentCultureIgnoreCase))
//            {
//                int count = 0;
//                foreach (var conversationReference in _conversationReferences)
//                {
//                    try
//                    {
//                        await messenger.SendMessage(message, conversationReference.Value, cancellationToken);
//                    }
//                    catch (Exception)
//                    {
//
//                    }
//
//
//                    count++;
//
//                    if (count > 20)
//                    {
//                        await Task.Delay(1000, cancellationToken);
//                        count = 0;
//                    }
//                }
//            }
//            else
//            {
//                var refs = _conversationReferences.FirstOrDefault(x => x.Key.Id == userId);
//                await messenger.SendMessage(message, refs.Value, cancellationToken);
//            }
//        }
//
//        private async Task ShowRating(ITurnContext stepcontext,
//            int take,
//            CancellationToken cancellationtoken)
//        {
//            var reply = stepcontext.Activity.CreateReply();
//            FormRatesToTelegramTable(reply, await GetRating(take));
//            await stepcontext.SendActivityAsync(reply, cancellationtoken);
//        }
//
//        private async Task<IEnumerable<KeyValuePair<string, int>>> GetRating(int take)
//        {
//            var teams = await _teamService.GetTeams();
//
//            string GetTeamNameById(string id)
//            {
//                var input = teams.FirstOrDefault(x => x.Id == id)?.Name ?? id;
//
//                string pattern = "[^A-Za-z0-9А-Яа-я]+";
//                string replacement = " ";
//
//                Regex regEx = new Regex(pattern);
//                string sanitized = Regex.Replace(regEx.Replace(input, replacement), @"\s+", " ");
//
//                return sanitized;
//            }
//
//            var ratedAnswers = _userService.CalcUserWeights(_scenarioService.Store);
//            return ratedAnswers
//                    .OrderByDescending(x => x.Value)
//                    .Take(take)
//                    .Select(x => new KeyValuePair<string, int>(GetTeamNameById(x.Key), x.Value))
//                ;
//        }
//
//
//
//        private void FormRatesToTelegramTable(IActivity reply, IEnumerable<KeyValuePair<string, int>> rates)
//        {
//            var htmlTable = new StringBuilder();
//
//            if (!rates.Any())
//            {
//                htmlTable.Append($"Еще нет ответов");
//            }
//
//            foreach (var pair in rates)
//            {
//                htmlTable.Append($"{pair.Key} -- {pair.Value}\n");
//            }
//
//            var channelData = new
//            {
//                method = "sendMessage",
//                parameters = new
//                {
//                    text = htmlTable.ToString(),
//                    parse_mode = "Markdown"
//                }
//            };
//
//            reply.ChannelData = JObject.FromObject(channelData);
//        }
    }
}
