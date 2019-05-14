using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Properties;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreBot.Dialogs
{
    public class MainDialog : CancelAndHelpDialog
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;

        public MainDialog(IConfiguration configuration, 
            ILogger<MainDialog> logger, 
            IScenarioService scenarioService,
            ITeamService teamService,
            IUserService userService)
            : base(nameof(MainDialog), scenarioService, userService, teamService)
        {
            _configuration = configuration;
            _logger = logger;
            _scenarioService = scenarioService ?? throw new System.ArgumentNullException(nameof(scenarioService));
            _userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
            _teamService = teamService;
            AddDialog(new SelectTeamDialog(teamService));
            AddDialog(new ScenarioDialog(scenarioService, userService, teamService));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                SelectTeamStepAsync,
                ScenarioLaunchStepAsync,
                //FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            
            // это на тот случай что человек уже себе поставил бота, но пользователя нет у нас в БД
            await _userService.GetOrCreateUser(stepContext.Context);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> SelectTeamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByAsync(stepContext.Context.Activity.ChannelId, stepContext.Context.Activity.From.Id);
            var teamId = _teamService.TryGetTeamId(user);
            if (teamId != null)
                return await stepContext.NextAsync(teamId, cancellationToken);
            return await stepContext.BeginDialogAsync(nameof(SelectTeamDialog), user, cancellationToken);
        }

        private async Task<DialogTurnResult> ScenarioLaunchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var teamId = (string)stepContext.Result;

            var scenarioDetails = _userService.GetLastScenarioDetailsExceptGameOver(teamId);

            if (scenarioDetails == null)
            {
                scenarioDetails = new ScenarioDetails()
                {
                    ScenarioId = "testScenario",
                    TeamId = teamId
                };
            }

            return await stepContext.BeginDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
        }

        //private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
        //    CancellationToken cancellationToken)
        //{
        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Квест окончен!"), cancellationToken);
        //    return await stepContext.EndDialogAsync(null, cancellationToken);
        //}
    }
}
