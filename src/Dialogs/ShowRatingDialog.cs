//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading;
//using System.Threading.Tasks;
//using CoreBot.Domain;
//using CoreBot.Service;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Schema;
//using Newtonsoft.Json.Linq;
//
//namespace CoreBot.Dialogs
//{
//    public class ShowRatingDialog : ComponentDialog
//    {
//        private readonly IScenarioService _scenarioService;
//        private readonly IUserService _userService;
//
//        public ShowRatingDialog(IScenarioService scenarioService, IUserService userService)
//            : base(nameof(ShowRatingDialog))
//        {
//            _scenarioService = scenarioService;
//            _userService = userService;
//            AddDialog(new TextPrompt(nameof(TextPrompt)));
//
//            var waterfallStep = new WaterfallStep[]
//            {
//                ShowRating,
//            };
//
//            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
//            InitialDialogId = nameof(WaterfallDialog);
//        }
//
//        private async Task<DialogTurnResult> ShowRating(WaterfallStepContext stepcontext,
//            CancellationToken cancellationtoken)
//        {
//            var reply = stepcontext.Context.Activity.CreateReply();
//            FormRatesToTelegramTable(reply, await GetRating());
//            await stepcontext.Context.SendActivityAsync(reply, cancellationtoken);
//
//            return await stepcontext.EndDialogAsync(null, cancellationtoken);
//        }
//
//        private async Task<IEnumerable<KeyValuePair<string, int>>> GetRating()
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
//                    .Take(10)
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
//    }
//}