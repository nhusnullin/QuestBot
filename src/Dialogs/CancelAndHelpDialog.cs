using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Exceptions;
using CoreBot.Properties;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Newtonsoft.Json.Linq;

namespace CoreBot.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;
        protected readonly ITeamService _teamService;
        protected readonly INotificationMessanger _notificationMessanger;
        protected ConcurrentDictionary<UserId, ConversationReference> _conversationReferences;
        public CancelAndHelpDialog(string id, IScenarioService scenarioService, IUserService userService, ITeamService teamService,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
            INotificationMessanger notificationMessanger)
            : base(id)
        {
            _scenarioService = scenarioService;
            _userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
            _teamService = teamService ?? throw new System.ArgumentNullException(nameof(teamService));
            _conversationReferences = conversationReferences ?? throw new System.ArgumentNullException(nameof(conversationReferences));
            _notificationMessanger = notificationMessanger ?? throw new System.ArgumentNullException(nameof(notificationMessanger));
            AddDialog(new ScenarioListDialog(scenarioService, userService, teamService, conversationReferences, notificationMessanger));
            AddDialog(new SetTeamNameDialog(nameof(SetTeamNameDialog), teamService, notificationMessanger, conversationReferences));
            AddDialog(new ShowRatingDialog(scenarioService, userService, teamService));
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
                        //case "2edaab42-9871-4c9b-8039-fd262121f8e0":
                        //    await _userService.DeleteUsers();
                        //    await _teamService.DeleteTeams();
                            //break;
                        case "top10":
                            await innerDc.BeginDialogAsync(nameof(ShowRatingDialog), null, cancellationToken);
                            return new DialogTurnResult(DialogTurnStatus.Waiting);
                        default:
                            if (text.StartsWith("top"))
                            {
                                var words = text.Split(" ").Where(x => !string.IsNullOrEmpty(x));

                                int.TryParse(words.Last(), out var count);

                                await ShowRating(innerDc.Context, count, cancellationToken);
                                return new DialogTurnResult(DialogTurnStatus.Waiting);
                            }

                            if (text.StartsWith("send"))
                            {
                                var words = text.Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToList();
                                string id = null;
                                string msg = "";

                                for(int i=0;i<words.Count; i++)
                                {
                                    if(i == 1)
                                    {
                                        id = words[i];
                                    }

                                    if(i > 1)
                                    {
                                        msg += " " + words[i];
                                    }
                                }

                                await SendTeamMessage(_notificationMessanger, msg, cancellationToken, id);
                                return new DialogTurnResult(DialogTurnStatus.Waiting);
                            }

                            return null;
                    }
                }
                catch(UserNotFoundException)
                {
                    await TurnContextExtensions.SendMessageAsync(innerDc.Context, Resources.PleaseWaitStartGame, cancellationToken);
                    return new DialogTurnResult(DialogTurnStatus.Waiting);
                }
                catch(AuthorizationException ex)
                {
                    await TurnContextExtensions.SendMessageAsync(innerDc.Context, ex.Message, cancellationToken);
                    return new DialogTurnResult(DialogTurnStatus.Waiting);
                }
            }

            return null;
        }

        public async Task SendTeamMessage(
            INotificationMessanger messenger,
            string message,
            CancellationToken cancellationToken,
            string userId
        )
        {
            if (string.Equals(userId, "toAll", StringComparison.CurrentCultureIgnoreCase))
            {
                int count = 0;
                foreach (var conversationReference in _conversationReferences)
                {
                    try
                    {
                        await messenger.SendMessage(message, conversationReference.Value, cancellationToken);
                    }
                    catch (Exception)
                    {

                    }


                    count++;

                    if (count > 20)
                    {
                        await Task.Delay(1000, cancellationToken);
                        count = 0;
                    }
                }
            }
            else
            {
                var refs = _conversationReferences.FirstOrDefault(x => x.Key.Id == userId);
                await messenger.SendMessage(message, refs.Value, cancellationToken);
            }
        }

        private async Task ShowRating(ITurnContext stepcontext,
            int take,
            CancellationToken cancellationtoken)
        {
            var reply = stepcontext.Activity.CreateReply();
            FormRatesToTelegramTable(reply, await GetRating(take));
            await stepcontext.SendActivityAsync(reply, cancellationtoken);
        }

        private async Task<IEnumerable<KeyValuePair<string, int>>> GetRating(int take)
        {
            var teams = await _teamService.GetTeams();

            string GetTeamNameById(string id)
            {
                var input = teams.FirstOrDefault(x => x.Id == id)?.Name ?? id;

                string pattern = "[^A-Za-z0-9А-Яа-я]+";
                string replacement = " ";

                Regex regEx = new Regex(pattern);
                string sanitized = Regex.Replace(regEx.Replace(input, replacement), @"\s+", " ");

                return sanitized;
            }

            var ratedAnswers = _userService.CalcUserWeights(_scenarioService.Store);
            return ratedAnswers
                    .OrderByDescending(x => x.Value)
                    .Take(take)
                    .Select(x => new KeyValuePair<string, int>(GetTeamNameById(x.Key), x.Value))
                ;
        }



        private void FormRatesToTelegramTable(IActivity reply, IEnumerable<KeyValuePair<string, int>> rates)
        {
            var htmlTable = new StringBuilder();

            if (!rates.Any())
            {
                htmlTable.Append($"Еще нет ответов");
            }

            foreach (var pair in rates)
            {
                htmlTable.Append($"{pair.Key} -- {pair.Value}\n");
            }

            var channelData = new
            {
                method = "sendMessage",
                parameters = new
                {
                    text = htmlTable.ToString(),
                    parse_mode = "Markdown"
                }
            };

            reply.ChannelData = JObject.FromObject(channelData);
        }
    }
}
